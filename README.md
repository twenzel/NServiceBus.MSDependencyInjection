# NServiceBus.MSDependencyInjection

[![Build status](https://ci.appveyor.com/api/projects/status/p1cfl6vw040xyw3c?svg=true)](https://ci.appveyor.com/project/twenzel/cake-markdowntopdf) [![NuGet Version](http://img.shields.io/nuget/v/NServiceBus.ObjectBuilder.MSDependencyInjection.svg?style=flat)](https://www.nuget.org/packages/NServiceBus.ObjectBuilder.MSDependencyInjection/) [![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Support for the Microsoft.Extensions.DependencyInjection container.

## Usage

```CSharp
public IServiceProvider ConfigureServices(IServiceCollection services)
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