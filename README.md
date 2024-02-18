# <img src="./Icon.png" height="25px" /> Canister

[![.NET Publish](https://github.com/JaCraig/Canister/actions/workflows/dotnet-publish.yml/badge.svg)](https://github.com/JaCraig/Canister/actions/workflows/dotnet-publish.yml)

Canister is one of the easiest ways to get IoC configuration under control. No longer do you have to search for that one class that you forgot to register. Instead use Canister to handle discovery and registration for you using a simple interface.

## Basic Usage

The system has a fairly simple interface and only a couple of functions that need explaining. The first is setup:

    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddCanisterModules();
    }

AddCanisterModules will automatically scan assemblies for modules and load them accordingly. Or if you're doing a desktop app:

    var Services = new ServiceCollection().AddCanisterModules();

Note that if you like, you can control which assemblies are searched:

    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddCanisterModules(configure => configure.AddAssembly(typeof(Startup).Assembly));
    }

It's recommended that you do this for security reasons as the default will search all assemblies found in the entry assembly's top level directory.

## Modules

Canister uses the concept of modules to wire things up. This allows you to place registration code in libraries that your system is using instead of worrying about it in every application. Simply add your library and Canister will automatically wire it up for you. In order to do this, under Canister.Interfaces there is the IModule interface. This interface, when implemented, has two items in it. The first is a property called Order. This determines the order that the modules are loaded in. The second is a function called Load:

    public class TestModule : IModule
    {
        public int Order => 1;

        public void Load(IServiceCollection bootstrapper)
        {
		    bootstrapper.AddAllTransient<IMyInterface>();
			bootstrapper.AddTransient<MyType>();
        }
    }
	
The module above is loaded automatically by the system and will have the Load function called at initialization time. At this point you should be able to resolve and register classes using the bootstrapper parameter. The service collection also has a couple of extra extension methods: AddAllTransient, AddAllScoped, AddAllSingleton:

    bootstrapper.AddAllTransient<IMyInterface>();
	
The AddAllxxxx functions will find everything that implements a class or interface in the Assemblies that you tell it to look in and will register them with the service collection.

### Canister Extension Methods

Canister provides a set of extension methods to streamline your IoC (Inversion of Control) container registration code. These methods offer convenient ways to conditionally register services based on certain criteria, enhancing the flexibility of your application's dependency injection setup.

#### 1. `AddTransientIf()`

The `AddTransientIf` method registers a service as transient only if a specified condition is met. This is useful when you want to dynamically determine whether a service should be transient or not.

```csharp
services.AddTransientIf<IMyService, MyService>(services => condition);
```

#### 2. `AddScopedIf()`

Similar to `AddTransientIf`, `AddScopedIf` registers a service as scoped based on a given condition.

```csharp
services.AddScopedIf<IMyScopedService, MyScopedService>(services => condition);
```

#### 3. `AddSingletonIf()`

The `AddSingletonIf` method registers a service as a singleton if the specified condition holds true.

```csharp
services.AddSingletonIf<IMySingletonService, MySingletonService>(services => condition);
```

#### 4. `AddKeyedTransientIf()`, `AddKeyedScopedIf()`, `AddKeyedSingletonIf()`

These methods follow the same pattern as their non-keyed counterparts but additionally allow you to register services with a specified key.

```csharp
services.AddKeyedTransientIf<IService>(key, implementationType, (services, key) => condition);
```

#### 5. `Exists()`

The `Exists` method checks whether a service with a specific type and, optionally, a key, has already been registered. This can be helpful in avoiding duplicate registrations or finding issues with your environment before starting the application.

```csharp
if (!services.Exists<IMyService>())
{
    services.AddTransient<IMyService, MyService>();
}
```

### Usage Example

Here's an example of how you might use these methods:

```csharp

IHostEnvironment? environment;

// Conditionally register a transient service if in development environment.
services.AddTransientIf<IMyService, MyDebugService>(_ => environment.IsDevelopment());

// However if you're in production, add a different implementation.
services.AddTransientIf<IMyService, MyProductionService>(_ => environment.IsProduction());

// Check if a keyed service is missing and log a warning if so.
if (!services.Exists<IService>(key))
{
    logger.LogWarning("Service {Service} is missing", key);
}

```

These methods empower you to create more dynamic and adaptive dependency injection configurations tailored to your application's requirements.

## Working With Other IoC Containers

While the library assumes you are using the built in ServiceCollection, it is possible to work with IoC containers. All that is required is that it implements the IServiceCollection interface.

## Using Canister in Your library

If you wish to use Canister in your library it is recommended that you build an extension method off of the ICanisterConfiguration interface that will allow you to register your needed assemblies for the user to make the experience a bit simpler when they want to control configuration themselves.

## Installation

The library is available via Nuget with the package name "Canister.IoC". To install it run the following command in the Package Manager Console:

Install-Package Canister.IoC

## Build Process

In order to build the library you may require the following:

1. Visual Studio 2022

Other than that, just clone the project and you should be able to load the solution and build without too much effort.
