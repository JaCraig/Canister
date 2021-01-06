# Canister

[![Build status](https://ci.appveyor.com/api/projects/status/9x5dp1v8qd1o3lii?svg=true)](https://ci.appveyor.com/project/JaCraig/canister)

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

        public void Load(IBootstrapper bootstrapper)
        {
		    bootstrapper.RegisterAll<IMyInterface>();
			bootstrapper.Register<MyType>();
        }
    }
	
The module above is loaded automatically by the system and will have the Load function called at initialization time. At this point you should be able to resolve and register classes using the bootstrapper parameter. The bootstrapper itself has only a couple of functions that you will need to deal with, namely Register, RegisterAll, Resolve, and ResolveAll:

    bootstrapper.Register<MyType>();
	
The Register function is how you add a class type to the system. You can register a Func to return the object, a static object, or just the type itself. You can also register them based on a string name as well as set the service lifetime for the item that you are registering if the underlying IoC container you're using supports it. Singleton, Scoped, or Transient scopes are supported.

    bootstrapper.RegisterAll<IMyInterface>();
	
RegisterAll will find everything that implements a class or interface in the Assemblies that you tell it to look for and will register them in the system.

    MyType MyObject = bootstrapper.Resolve<MyType>();
	
The Resolve function resolves an individual item of the type specified based on the information that you pass into the function.

    IEnumerable<IMyInterface> MyObjects = bootstrapper.ResolveAll<IMyInterface>();
	
The ResolveAll function, on the other hand, will resolve all objects that are associated with the class or interface specified.

## Generic types

It is possible to register open generic types in the system:

    bootstrapper.Register(typeof(GenericExampleClass<>));

This class can then be resolved by supplying a closed generic type to the Resolve function:

    GenericExampleClass<AnotherClass> MyObject = bootstrapper.Resolve(typeof(GenericExampleClass<AnotherClass>)) as GenericExampleClass<AnotherClass>;

## Wrapping Other IoC Containers

While the system has a default bootstrapper for ServiceCollection, it is possible to wrap other IoC containers. In order to do this, simply wrap the other IoC container in a class that implements the Canister.Interfaces.IBootstrapper interface. You may also like to use the Canister.BaseClasses.BootstrapperBase abstract class but is not required by the system. The system will then automatically detect the bootstrapper in your code when initializing the system and use that instead of the default IoC container. However you must register/resolve the modules within the Builder method as it does not happen automatically.

## Using Canister in Your library

If you wish to use Canister in your library it is recommended that you build an extension method off of the ICanisterConfiguration interface that will allow you to register your needed assemblies for the user to make the experience a bit simpler when they want to control configuration themselves.

## Installation

The library is available via Nuget with the package name "Canister.IoC". To install it run the following command in the Package Manager Console:

Install-Package Canister.IoC

## Build Process

In order to build the library you may require the following:

1. Visual Studio 2019

Other than that, just clone the project and you should be able to load the solution and build without too much effort.
