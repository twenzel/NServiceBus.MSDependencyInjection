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

            if (settings.TryGet(out containerHolder))
            {
                settings.AddStartupDiagnosticsSection("NServiceBus.Extensions.DependencyInjection", new
                {
                    UsingExistingCollection = true
                });

                return new ServicesObjectBuilder(containerHolder.ExistingCollection);
            }

            settings.AddStartupDiagnosticsSection("NServiceBus.Extensions.DependencyInjection", new
            {
                UsingExistingCollection = false
            });

            return new ServicesObjectBuilder();
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
