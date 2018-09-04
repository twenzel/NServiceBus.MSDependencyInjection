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
    [NonParallelizable]
    public class When_registering_components_transient : NServiceBusAcceptanceTest
    {
        [Test]
        public async Task Should_Be_Created_Everytime()
        {
            var context = await Scenario.Define<Context>()                
                .WithEndpoint<EndpointWithTransientDependency>(b =>
                {                    
                    b.When((bus, c) => bus.SendLocal(new MyMessage()));
                    b.When((bus, c) => bus.SendLocal(new MyMessage()));
                })
                .Done(c => c.WasCalledTimes >= 2)
                .Run().ConfigureAwait(false);

            Assert.AreEqual(2, context.HandlerCreationAmount);
            Assert.AreEqual(4, MyDependency.CreationAmount);
            Assert.AreEqual(2, MyAnotherDependency.CreationAmount);
        }


        public class Context : ScenarioContext
        {
            public int WasCalledTimes { get; set; }
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
                    services.AddTransient(typeof(MyAnotherDependency));

                    SetServices(services);
                }
            }
        }

        public class MyMessageHandler : IHandleMessages<MyMessage>
        {
            private Context _testContext;
            private readonly MyDependency _myDependency;
            private readonly MyAnotherDependency _anotherDependency;

            public MyMessageHandler(Context testContext, MyDependency myDependency, MyAnotherDependency anotherDependency)
            {
                _testContext = testContext;
                _myDependency = myDependency;
                _testContext.HandlerCreationAmount++;
                _anotherDependency = anotherDependency;
            }

            public Task Handle(MyMessage message, IMessageHandlerContext context)
            {
                _testContext.WasCalledTimes++;
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

        public class MyAnotherDependency
        {
            public static int CreationAmount { get; set; }
            private readonly MyDependency _myDependency;

            public MyAnotherDependency(MyDependency dependency)
            {
                CreationAmount++;
                _myDependency = dependency;
            }
        }
    }

    [NonParallelizable]
    public class When_registering_components_scoped : NServiceBusAcceptanceTest
    {
       
        [Test]
        public async Task Should_Be_Created_OnEachScope()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<EndpointWithScopedDependency>(b =>
                {
                    b.When((bus, c) => bus.SendLocal(new MyMessage()));
                    b.When((bus, c) => bus.SendLocal(new MyMessage()));
                })
                .Done(c => c.WasCalledTimes >= 2)
                .Run().ConfigureAwait(false);

            Assert.AreEqual(2, context.HandlerCreationAmount);
            Assert.AreEqual(2, MyDependency.CreationAmount);
            Assert.AreEqual(2, MyAnotherDependency.CreationAmount);
        }     

        public class Context : ScenarioContext
        {
            public int WasCalledTimes { get; set; }
            public int HandlerCreationAmount { get; set; }
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
                    services.AddTransient(typeof(MyAnotherDependency));

                    SetServices(services);
                }
            }
        }       

        public class MyMessageHandler : IHandleMessages<MyMessage>
        {
            private Context _testContext;
            private readonly MyDependency _myDependency;
            private readonly MyAnotherDependency _anotherDependency;

            public MyMessageHandler(Context testContext, MyDependency myDependency, MyAnotherDependency anotherDependency)
            {
                _testContext = testContext;
                _myDependency = myDependency;
                _testContext.HandlerCreationAmount++;
                _anotherDependency = anotherDependency;
            }

            public Task Handle(MyMessage message, IMessageHandlerContext context)
            {
                _testContext.WasCalledTimes++;
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

        public class MyAnotherDependency
        {
            public static int CreationAmount { get; set; }
            private readonly MyDependency _myDependency;

            public MyAnotherDependency(MyDependency dependency)
            {
                CreationAmount++;
                _myDependency = dependency;
            }
        }
    }

    public class When_registering_components_singleton : NServiceBusAcceptanceTest
    {
       
        [Test]
        public async Task Should_Be_Created_Once()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<EndpointWithSingletonDependency>(b =>
                {
                    b.When((bus, c) => bus.SendLocal(new MyMessage()));
                    b.When((bus, c) => bus.SendLocal(new MyMessage()));
                })
                .Done(c => c.WasCalledTimes >= 2)
                .Run().ConfigureAwait(false);

            Assert.AreEqual(2, context.HandlerCreationAmount);
            Assert.AreEqual(1, MyDependency.CreationAmount);
            Assert.AreEqual(2, MyAnotherDependency.CreationAmount);
            
        }

        public class Context : ScenarioContext
        {
            public int WasCalledTimes { get; set; }
            public int HandlerCreationAmount { get; set; }
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
                    services.AddTransient(typeof(MyAnotherDependency));

                    SetServices(services);
                }
            }
        }

        public class MyMessageHandler : IHandleMessages<MyMessage>
        {
            private Context _testContext;
            private readonly MyDependency _myDependency;
            private readonly MyAnotherDependency _anotherDependency;

            public MyMessageHandler(Context testContext, MyDependency myDependency, MyAnotherDependency anotherDependency)
            {
                _testContext = testContext;
                _myDependency = myDependency;
                _testContext.HandlerCreationAmount++;
                _anotherDependency = anotherDependency;
            }

            public Task Handle(MyMessage message, IMessageHandlerContext context)
            {
                _testContext.WasCalledTimes++;
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

        public class MyAnotherDependency
        {
            public static int CreationAmount { get; set; }
            private readonly MyDependency _myDependency;

            public MyAnotherDependency(MyDependency dependency)
            {
                CreationAmount++;
                _myDependency = dependency;
            }
        }
    }
}
