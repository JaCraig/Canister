# Canister

[![Build status](https://ci.appveyor.com/api/projects/status/9x5dp1v8qd1o3lii?svg=true)](https://ci.appveyor.com/project/JaCraig/canister)

Canister is a simple Inversion of Control container wrapper. It can be used by itself or as a wrapper around other IoC implementations giving them a common interface to deal with.

## Basic Usage

The system has a fairly simple interface and only a couple of functions that need explaining. The first is setup:

    using (IBootstrapper Bootstrapper = Canister.Builder.CreateContainer(new List<ServiceDescriptor>())
                                                        .AddAssembly(typeof(MyClass).Assembly)
                                                        .AddAssembly(typeof(IMyInterface).Assembly)
                                                        .Build())
	{
		...
	}
	
The CreateContainer function initializes the IoC container and returns it. If using ASP.Net MVC Core, it accepts the current ServiceDescriptors from the built in IoC container and adds them to the system. You may also pass in other assemblies that you want it to search in for modules and classes as either parameters or by calling AddAssembly on the IBootstrapper object that is returned. Finally calling Build() on the IBootstrapper will register and call any IModules that are found in the assemblies specified. Once created it can be accessed here:

    IBootstrapper Bootstrapper = Canister.Builder.Bootstrapper;
	
The bootstrapper itself has only a couple of functions that you will need to deal with, namely Register, RegisterAll, Resolve, and ResolveAll:

    Canister.Builder.Bootstrapper.Register<MyType>();
	
The Register function is how you add a class type to the system. You can register a Func to return the object, a static object, or just the type itself. You can also register them based on a string name as well as set the service lifetime for the item that you are registering. Either Singleton, Scoped, or Transient. The system will also attempt to resolve all paramters needed for the constructor. It will attempt to call the constructor that it has the most types for. For instance if there is a constructor with one paramter with the one type registered and one constructor with two parameters registered, it will call the constructor with two parameters.

    Canister.Builder.Bootstrapper.RegisterAll<IMyInterface>();
	
RegisterAll on the other hand will find everything that implements a class or interface in the Assemblies that you tell it to look in and will register them in the system.

    MyType MyObject = Canister.Builder.Bootstrapper.Resolve<MyType>();
	
The Resolve function resolves an individual item of the type specified based on the information that you pass into the function. In the above instance it just asks for the default factory for creating MyType objects.

    IEnumerable<IMyInterface> MyObjects = Canister.Builder.Bootstrapper.ResolveAll<IMyInterface>();
	
The ResolveAll function, on the other hand, will resolve all objects that are associated with the class or interface specified. Generally speaking you would use this in conjunction with RegisterAll.

## Generic types

It is possible to register open generic types in the system:

    Canister.Builder.Bootstrapper.Register(typeof(GenericExampleClass<>));

This class can then be resolved by supplying a closed generic type to the Resolve function:

    GenericExampleClass<AnotherClass> MyObject=Canister.Builder.Bootstrapper.Resolve(typeof(GenericExampleClass<AnotherClass>)) as GenericExampleClass<AnotherClass>;

## Modules

Generally speaking though you should register and wire up your objects by using modules. Under Canister.Interfaces there is the IModule interface. This interface, when implemented, has two items in it. The first is a property called Order. This determines the order that the modules are loaded in. The second is a function called Load:

    public class TestModule : IModule
    {
        public int Order => 1;

        public void Load(IBootstrapper bootstrapper)
        {
		    bootstrapper.RegisterAll<IMyInterface>();
			bootstrapper.Register<MyType>();
        }
    }
	
The module above is loaded automatically by the system and will have the Load function called at initialization time. At this point you should be able to resolve and register classes using the bootstrapper parameter. Usage of this is encouraged as it allows your systems to be more modular, instead of requiring all of your registration in one location.

## Wrapping Other IoC Containers

While the system has a default bootstrapper, it is possible to wrap other IoC containers. In order to do this, simply wrap the other IoC container in a class that implements the Canister.Interfaces.IBootstrapper interface. You may also like to use the Canister.BaseClasses.BootstrapperBase abstract class but is not required by the system. The system will then automatically detect the bootstrapper in your code when initializing the system and use that instead of the default IoC container. However you must register/resolve the modules within the Builder method as it does not happen automatically.

## Using Canister in Your library

If you wish to use Canister in your library it is recommended that you build an extension method off of the IBootstrapper interface that will allow you to register your needed assemblies for the user to make the experience a bit simpler.

## Installation

The library is available via Nuget with the package name "Canister.IoC". To install it run the following command in the Package Manager Console:

Install-Package Canister.IoC

## Build Process

In order to build the library you may require the following:

1. Visual Studio 2017

Other than that, just clone the project and you should be able to load the solution and build without too much effort.
