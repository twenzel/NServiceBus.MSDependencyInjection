using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.AcceptanceTesting;
using NServiceBus.AcceptanceTests;
using NServiceBus.AcceptanceTests.EndpointTemplates;
using NUnit.Framework;

namespace NServiceBus.MSDependencyInjection.AcceptanceTests
{
    public class When_registering_components : NServiceBusAcceptanceTest
    {
        [Test]
        public async Task With_Transient_Component_Should_Be_Created_Everytime()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<EndpointWithTransientDependency>(b =>
                {
                    b.When((bus, c) => bus.SendLocal(new MyMessage()));
                    b.When((bus, c) => bus.SendLocal(new MyMessage()));
                })
                .Done(c => c.WasCalled)
                .Run().ConfigureAwait(false);

            Assert.AreEqual(2, context.HandlerCreationAmount);
            Assert.AreEqual(2, MyDependency.CreationAmount);
            
        }

        public class Context : ScenarioContext
        {
            public bool WasCalled { get; set; }
            public int HandlerCreationAmount { get; set; }            
        }

        public class EndpointWithTransientDependency : EndpointConfigurationBuilder
        {
            public EndpointWithTransientDependency()
            {
                EndpointSetup<DefaultServerWithTransientDependency>();
            }

            class DefaultServerWithTransientDependency : DefaultServer
            {
                public DefaultServerWithTransientDependency()
                {
                    var services = new ServiceCollection();
                    services.AddTransient(typeof(MyDependency));

                    SetServices(services);
                }
            }
        }

        public class EndpointWithScopedDependency : EndpointConfigurationBuilder
        {
            public EndpointWithScopedDependency()
            {
                EndpointSetup<DefaultServerWithScopedDependency>();
            }

            class DefaultServerWithScopedDependency : DefaultServer
            {
                public DefaultServerWithScopedDependency()
                {
                    var services = new ServiceCollection();
                    services.AddScoped(typeof(MyDependency));

                    SetServices(services);
                }
            }
        }

        public class EndpointWithSingletonDependency : EndpointConfigurationBuilder
        {
            public EndpointWithSingletonDependency()
            {
                EndpointSetup<DefaultServerWithSingletonDependency>();
            }

            class DefaultServerWithSingletonDependency : DefaultServer
            {
                public DefaultServerWithSingletonDependency()
                {
                    var services = new ServiceCollection();
                    services.AddSingleton(typeof(MyDependency));

                    SetServices(services);
                }
            }
        }

        public class MyMessageHandler : IHandleMessages<MyMessage>
        {
            private Context _testContext;
            private readonly MyDependency _myDependency;

            public MyMessageHandler(Context testContext, MyDependency myDependency)
            {
                _testContext = testContext;
                _myDependency = myDependency;
                _testContext.HandlerCreationAmount++;
            }

            public Task Handle(MyMessage message, IMessageHandlerContext context)
            {
                _testContext.WasCalled = true;
                return Task.FromResult(0);
            }
        }

        public class MyMessage : ICommand
        {
        }

        public class MyDependency
        {
            public static int CreationAmount { get; set; }

            public MyDependency()
            {
                CreationAmount++;
            }
        }
    }
}
