using NServiceBus.AcceptanceTesting;
using NServiceBus.AcceptanceTests;
using NServiceBus.AcceptanceTests.EndpointTemplates;
using NUnit.Framework;
using System.Threading.Tasks;

namespace NServiceBus.MSDependencyInjection.AcceptanceTests
{
    public class When_setting_properties_explicitly_via_the_container : NServiceBusAcceptanceTest
    {
        [Test]
        public async Task Should_use_provide_values()
        {
            var simpleValue = "SomeValue";

            var context = await Scenario.Define<Context>()
                .WithEndpoint<Endpoint>(b =>
                {
                    b.CustomConfig(c =>
                    {
                        c.SendFailedMessagesTo("error");
                        c.RegisterComponents(r => r.ConfigureComponent(builder => new Endpoint.MyMessageHandler(builder.Build<Context>())
                        {
                            MySimpleDependency = simpleValue
                        }, DependencyLifecycle.InstancePerCall));
                    });
                    b.When((bus, c) => bus.SendLocal(new MyMessage()));
                })
                .Done(c => c.WasCalled)
                .Run().ConfigureAwait(false);

            Assert.AreEqual(simpleValue, context.PropertyValue);
        }

        public class Context : ScenarioContext
        {
            public bool WasCalled { get; set; }
            public string PropertyValue { get; set; }
        }

        public class Endpoint : EndpointConfigurationBuilder
        {
            public Endpoint()
            {
                EndpointSetup<DefaultServer>();
            }

            public class MyMessageHandler : IHandleMessages<MyMessage>
            {
                private Context _testContext;

                public string MySimpleDependency { get; set; }

                public MyMessageHandler(Context testContext)
                {
                    _testContext = testContext;
                }

                public Task Handle(MyMessage message, IMessageHandlerContext context)
                {
                    _testContext.PropertyValue = MySimpleDependency;
                    _testContext.WasCalled = true;
                    return Task.FromResult(0);
                }
            }

            public class MyPropDependency
            {
            }
        }

        public class MyMessage : ICommand
        {
        }
    }
}
