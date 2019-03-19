using System;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Container;
using NServiceBus.ObjectBuilder.Common;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using NServiceBus.Settings;

namespace NServiceBus
{
    /// <summary>
    /// Services builder
    /// </summary>
    public class ServicesBuilder : ContainerDefinition
    {
        /// <summary>
        /// Implementers need to new up a new container.
        /// </summary>
        /// <param name="settings">The settings to check if an existing container exists.</param>
        /// <returns>The new container wrapper.</returns>
        public override IContainer CreateContainer(ReadOnlySettings settings)
        {
            CollectionHolder containerHolder;
            Func<IServiceCollection, UpdateableServiceProvider> serviceProviderFactory = null;

            if (!settings.TryGet(out serviceProviderFactory))
            {
                serviceProviderFactory = sc => new UpdateableServiceProvider(sc);
            }

            if (settings.TryGet(out containerHolder))
            {
                settings.AddStartupDiagnosticsSection("NServiceBus.Extensions.DependencyInjection", new
                {
                    UsingExistingCollection = true
                });

                return new ServicesObjectBuilder(containerHolder.ExistingCollection, serviceProviderFactory);
            }

            settings.AddStartupDiagnosticsSection("NServiceBus.Extensions.DependencyInjection", new
            {
                UsingExistingCollection = false
            });

            return new ServicesObjectBuilder(serviceProviderFactory);
        }

        internal class CollectionHolder
        {
            public CollectionHolder(IServiceCollection services)
            {
                ExistingCollection = services;
            }

            public IServiceCollection ExistingCollection { get; }
        }
    }
}
