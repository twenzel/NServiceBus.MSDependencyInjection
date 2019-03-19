using System;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Container;
using NServiceBus.ObjectBuilder.MSDependencyInjection;

namespace NServiceBus
{
    /// <summary>
    /// Services extension to pass an existing service instances.
    /// </summary>
    public static class ServicesExtensions
    {
        /// <summary>
        /// Use a pre-configured service collection.
        /// </summary>
        /// <param name="customizations"></param>
        /// <param name="services">The existing service collection.</param>
        public static void ExistingServices(this ContainerCustomizations customizations, IServiceCollection services)
        {
            customizations.Settings.Set<ServicesBuilder.CollectionHolder>(new ServicesBuilder.CollectionHolder(services));
        }

        public static void ServiceProviderFactory(this ContainerCustomizations customizations, Func<IServiceCollection, UpdateableServiceProvider> serviceProviderFactory)
        {
            customizations.Settings.Set<Func<IServiceCollection, UpdateableServiceProvider>>(serviceProviderFactory);
        }
    }
}
