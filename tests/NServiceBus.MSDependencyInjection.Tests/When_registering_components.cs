using Microsoft.Extensions.DependencyInjection;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using NUnit.Framework;

namespace NServiceBus.MSDependencyInjection.Tests
{
    [TestFixture]
    public class When_registering_components
    {
        [Test]
        public void New_component_can_be_resolved_when_depending_on_component_from_existing_collection()
        {
            var services = new ServiceCollection();
            services.AddTransient(typeof(DependentType));

            using (var builder = new ServicesObjectBuilder(services))
            {
                builder.Configure(typeof(RequestingType), DependencyLifecycle.InstancePerCall);

                Assert.IsNotNull(builder.Build(typeof(RequestingType)));
            }
        }

        class DependentType
        {

        }

        class RequestingType
        {
            public RequestingType(DependentType dependent)
            {

            }
        }
    }
}
