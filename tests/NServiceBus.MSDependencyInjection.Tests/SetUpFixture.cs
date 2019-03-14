using NServiceBus.ContainerTests;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using NUnit.Framework;

[SetUpFixture]
public class SetUpFixture
{
    public SetUpFixture()
    {
        TestContainerBuilder.ConstructBuilder = () => new ServicesObjectBuilder();
    }
}
