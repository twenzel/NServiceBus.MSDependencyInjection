using NServiceBus.ContainerTests;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using NUnit.Framework;

namespace NServiceBus.MSDependencyInjection.Tests
{
    [SetUpFixture]
    public class SetUpFixture
    {
        public SetUpFixture()
        {
            TestContainerBuilder.ConstructBuilder = () => new ServicesObjectBuilder();
        }
    }
}