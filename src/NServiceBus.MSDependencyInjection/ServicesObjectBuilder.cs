using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Logging;
using NServiceBus.ObjectBuilder.Common;

namespace NServiceBus.ObjectBuilder.MSDependencyInjection
{
    internal class ServicesObjectBuilder : IContainer
    {
        private bool _isChild;
        private static ILog s_logger = LogManager.GetLogger<ServicesObjectBuilder>();
        private readonly bool _owned;
        private readonly UpdateableServiceProvider _runtimeServiceProvider;
        private readonly IServiceCollection _services;

        public ServicesObjectBuilder() : this(new ServiceCollection(), true)
        {
        }

        public ServicesObjectBuilder(IServiceCollection services) : this(services, false)
        {
        }

        public ServicesObjectBuilder(IServiceCollection services, bool owned)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services), "The object builder must be initialized with a valid service collection instance.");

            _owned = owned;
            _runtimeServiceProvider = new UpdateableServiceProvider(services);
            _services = services;

            Initialize();
        }

        private ServicesObjectBuilder(bool owned, UpdateableServiceProvider updateableServiceProvider)
        {
            _owned = owned;
            _runtimeServiceProvider = updateableServiceProvider;

            Initialize();
        }

        public void Dispose()
        {
            //Injected at compile time
        }

        public void DisposeManaged()
        {
            //if we are in a child scope dispose of that but not the parent container
            if (!_isChild && _owned)
            {
                _runtimeServiceProvider.Dispose();
            }

            if (_owned)
            {
                (_services as IDisposable)?.Dispose();
            }
        }

        public IContainer BuildChildContainer()
        {
            return new ServicesObjectBuilder(false, _runtimeServiceProvider)
            {
                _isChild = true,
            };
        }

        public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
        {
            ThrowIfCalledOnChildContainer();

            if (HasComponent(component))
            {
                s_logger.Info("Component " + component.FullName + " was already registered in the container.");
                return;
            }

            var lifestyle = GetLifetimeTypeFrom(dependencyLifecycle);
            var services = GetAllServiceTypesFor(component);

            _runtimeServiceProvider.AddServices(services, component, lifestyle);
        }

        public void Configure<T>(Func<T> component, DependencyLifecycle dependencyLifecycle)
        {
            ThrowIfCalledOnChildContainer();

            var componentType = typeof(T);

            if (HasComponent(componentType))
            {
                s_logger.Info("Component " + componentType.FullName + " was already registered in the container.");
                return;
            }

            var lifestyle = GetLifetimeTypeFrom(dependencyLifecycle);
            var services = GetAllServiceTypesFor(componentType);

            _runtimeServiceProvider.AddServices(services, component, lifestyle);
        }

        public void RegisterSingleton(Type lookupType, object instance)
        {
            ThrowIfCalledOnChildContainer();

            var serviceDescriptor = _runtimeServiceProvider.FirstOrDefault(d => d.ServiceType == lookupType);

            if (serviceDescriptor != null)
                _runtimeServiceProvider.Remove(serviceDescriptor);

            _runtimeServiceProvider.AddSingleton(lookupType, instance);
        }

        /// <summary>
        /// Starts a new scope of the component lifetime
        /// </summary>
        public void BeginScope()
        {
            _runtimeServiceProvider.BeginScope();
        }

        /// <summary>
        /// Ends the current scope
        /// </summary>
        public void EndScope()
        {
            _runtimeServiceProvider.EndScope();
        }

        public object Build(Type typeToBuild)
        {
            return _runtimeServiceProvider.GetService(typeToBuild);
        }

        public IEnumerable<object> BuildAll(Type typeToBuild)
        {
            return _runtimeServiceProvider.GetServices(typeToBuild);
        }

        public bool HasComponent(Type componentType)
        {
            return _runtimeServiceProvider.Any(d => d.ServiceType == componentType);
        }

        public void Release(object instance)
        {
            // no release logic
        }

        private void Initialize()
        {
            Configure(typeof(UnitOfWorkScope), DependencyLifecycle.InstancePerCall);
            Configure<ServicesObjectBuilder>(() => this, DependencyLifecycle.InstancePerCall);
        }

        void ThrowIfCalledOnChildContainer()
        {
            if (_isChild)
            {
                throw new InvalidOperationException("Reconfiguration of child containers is not allowed.");
            }
        }

        static ServiceLifetime GetLifetimeTypeFrom(DependencyLifecycle dependencyLifecycle)
        {
            switch (dependencyLifecycle)
            {
                case DependencyLifecycle.InstancePerCall:
                    return ServiceLifetime.Transient;
                case DependencyLifecycle.SingleInstance:
                    return ServiceLifetime.Singleton;
                case DependencyLifecycle.InstancePerUnitOfWork:
                    return ServiceLifetime.Scoped;
            }

            throw new ArgumentException("Unhandled lifecycle - " + dependencyLifecycle);
        }

        static IEnumerable<Type> GetAllServiceTypesFor(Type t)
        {
            return t.GetInterfaces()
                .Where(x => !x.FullName.StartsWith("System."))
                .Concat(new[] { t });
        }
    }
}
