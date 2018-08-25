using Microsoft.Extensions.DependencyInjection;
using NServiceBus.AcceptanceTesting;
using NServiceBus.AcceptanceTests;
using NServiceBus.AcceptanceTests.EndpointTemplates;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace NServiceBus.MSDependencyInjection.AcceptanceTests
{
    public class When_using_externally_owned_container : NServiceBusAcceptanceTest
    {
        [Test]
        public async Task Should_shutdown_properly()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<Endpoint>()
                .Done(c => c.EndpointsStarted)
                .Run().ConfigureAwait(false);

            Assert.IsFalse(context.Decorator.Disposed);
            Assert.DoesNotThrow(() => (context.Container as IDisposable)?.Dispose());
        }

        class Context : ScenarioContext
        {
            public IServiceCollection Container { get; set; }
            public ServiceCollectionDecorator Decorator { get; set; }
        }

        class Endpoint : EndpointConfigurationBuilder
        {
            public Endpoint()
            {
                EndpointSetup<DefaultServer>((config, desc) =>
                {
                    var services = new ServiceCollection();
                    var decorator = new ServiceCollectionDecorator(services);

                    config.SendFailedMessagesTo("error");
                    config.UseContainer<ServicesBuilder>(c => c.ExistingServices(services));

                    var context = (Context)desc.ScenarioContext;
                    context.Container = services;
                    context.Decorator = decorator;
                });
            }
        }
    }
}
