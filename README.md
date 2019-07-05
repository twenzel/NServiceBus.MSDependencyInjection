# NServiceBus.MSDependencyInjection

[![Build status](https://ci.appveyor.com/api/projects/status/p1cfl6vw040xyw3c?svg=true)](https://ci.appveyor.com/project/twenzel/nservicebus-msdependencyinjection) [![NuGet Version](http://img.shields.io/nuget/v/NServiceBus.MSDependencyInjection.svg?style=flat)](https://www.nuget.org/packages/NServiceBus.MSDependencyInjection/) [![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Support for the Microsoft.Extensions.DependencyInjection container.

## Usage

```CSharp
public void ConfigureServices(IServiceCollection services)
{
    // register any services to IServiceCollection
    // ...

    var endpointConfiguration = new EndpointConfiguration("Sample.Core");
    endpointConfiguration.UseTransport<LearningTransport>();
    endpointConfiguration.UseContainer<ServicesBuilder>(customizations =>
        {
            customizations.ExistingServices(services);
        });

    endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
}
```

To share the used service provide across (ASP) .NET Core and NServiceBus (recommended) you need to return the created service provider.
```CSharp
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    // register any services to IServiceCollection
    // ...

    var endpointConfiguration = new EndpointConfiguration("Sample.Core");
    endpointConfiguration.UseTransport<LearningTransport>();

    UpdateableServiceProvider container = null;
    endpointConfiguration.UseContainer<ServicesBuilder>(customizations =>
        {
            customizations.ExistingServices(services);
            customizations.ServiceProviderFactory(sc => 
            {
                container = new UpdateableServiceProvider(sc);
                return container;
            });
        });

    endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

    return container;
}
```

If you need to register the `IEndpointInstance` also to the service collection please use following pattern:

```CSharp
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    ...
    IEndpointInstance endpointInstance = null;

    services.AddSingleton<IMessageSession>(sp => endpointInstance);

    endpointConfiguration.UseContainer<ServicesBuilder>(c => c.ExistingServices(services));

    endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
}
```