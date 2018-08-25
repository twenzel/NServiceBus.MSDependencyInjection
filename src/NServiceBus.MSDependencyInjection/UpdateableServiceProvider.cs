using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NServiceBus.ObjectBuilder.MSDependencyInjection
{
    internal class UpdateableServiceProvider : IServiceProvider, IServiceCollection, IDisposable
    {
        private IServiceProvider _serviceProvider;
        private readonly ServiceCollection _services;

        public int Count => _services.Count;

        public bool IsReadOnly => _services.IsReadOnly;

        public ServiceDescriptor this[int index] { get => _services[index]; set => _services[index] = value; }

        public UpdateableServiceProvider(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            _services = new ServiceCollection();

            // Import existing services
            foreach (var service in services)
                ((IServiceCollection)_services).Add(service);

            // create initial service provider
            UpdateServiceProvider();
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        public int IndexOf(ServiceDescriptor item)
        {
            return _services.IndexOf(item);
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            _services.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _services.RemoveAt(index);
        }

        public void Add(ServiceDescriptor item)
        {
            ((IServiceCollection)_services).Add(item);
            UpdateServiceProvider();
        }

        public void AddServices(IEnumerable<Type> services, Type component, ServiceLifetime lifetime)
        {
            foreach (var service in services)
                ((IServiceCollection)_services).Add(new ServiceDescriptor(service, component, lifetime));

            UpdateServiceProvider();
        }

        public void AddServices<T>(IEnumerable<Type> services, Func<T> factory, ServiceLifetime lifetime)
        {
            foreach (var service in services)
                ((IServiceCollection)_services).Add(new ServiceDescriptor(service, (s) => factory(), lifetime));

            UpdateServiceProvider();
        }

        public void Clear()
        {
            _services.Clear();
            UpdateServiceProvider();
        }

        public bool Contains(ServiceDescriptor item)
        {
            return _services.Contains(item);
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            _services.CopyTo(array, arrayIndex);
        }

        public bool Remove(ServiceDescriptor item)
        {
            if (_services.Remove(item))
            {
                UpdateServiceProvider();
                return true;
            }

            return false;
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return _services.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _services.GetEnumerator();
        }

        private void UpdateServiceProvider()
        {
            _serviceProvider = _services.BuildServiceProvider();
        }

        public void Dispose()
        {
            // Injected at compile time
        }

    }
}
