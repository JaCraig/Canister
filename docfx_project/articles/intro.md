# Code

[!code-csharp[](../../Canister.IoC.Example/Program.cs)]

# Output

```
Select an example to run:
1. Basic module registration and resolution
2. Using UseLogger with Canister configuration
Q. Quit
Enter your choice: 1
--- Example 1: Basic module registration and resolution ---
Number of registered classes found: 2
Number of services found: 2
ExampleService1
ExampleService2
SimpleExampleClass

Select an example to run:
1. Basic module registration and resolution
2. Using UseLogger with Canister configuration
Q. Quit
Enter your choice: 2
--- Example 2: Using UseLogger with Canister configuration ---
info: Canister.IoC.Example[0]
      Default assemblies added: Canister.IoC.Example, Canister.IoC
info: Canister.IoC.Example[0]
      Assembly loaded: Canister.IoC
info: Canister.IoC.Example[0]
      Assembly loaded: Canister.IoC.Example
info: Canister.IoC.Example[0]
      Assembly loaded: Fast.Activator
info: Canister.IoC.Example[0]
      Assembly loaded: Microsoft.Extensions.Configuration.Abstractions
info: Canister.IoC.Example[0]
      Assembly loaded: Microsoft.Extensions.Configuration.Binder
info: Canister.IoC.Example[0]
      Assembly loaded: Microsoft.Extensions.Configuration
info: Canister.IoC.Example[0]
      Assembly loaded: Microsoft.Extensions.DependencyInjection.Abstractions
info: Canister.IoC.Example[0]
      Assembly loaded: Microsoft.Extensions.DependencyInjection
info: Canister.IoC.Example[0]
      Assembly loaded: Microsoft.Extensions.Logging.Abstractions
info: Canister.IoC.Example[0]
      Assembly loaded: Microsoft.Extensions.Logging.Configuration
info: Canister.IoC.Example[0]
      Assembly loaded: Microsoft.Extensions.Logging.Console
info: Canister.IoC.Example[0]
      Assembly loaded: Microsoft.Extensions.Logging
info: Canister.IoC.Example[0]
      Assembly loaded: Microsoft.Extensions.Options.ConfigurationExtensions
info: Canister.IoC.Example[0]
      Assembly loaded: Microsoft.Extensions.Options
info: Canister.IoC.Example[0]
      Assembly loaded: Microsoft.Extensions.Primitives
info: Canister.IoC.Example[0]
      Assembly loaded: System.Diagnostics.DiagnosticSource
info: Canister.IoC.Example[0]
      Assembly loaded: System.IO.Pipelines
info: Canister.IoC.Example[0]
      Assembly loaded: System.Text.Encodings.Web
info: Canister.IoC.Example[0]
      Assembly loaded: System.Text.Json
info: Canister.IoC.Example[0]
      Adding transient service: Canister.IoC.Example.MyModule as Canister.Interfaces.IModule
info: Canister.IoC.Example[0]
      Adding transient service: Canister.IoC.Modules.DefaultModule as Canister.Interfaces.IModule
info: Canister.IoC.Example[0]
      Module loading: MyModule
info: Canister.IoC.Example[0]
      Module successfully loaded: MyModule
info: Canister.IoC.Example[0]
      Module loading: DefaultModule
info: Canister.IoC.Example[0]
      Module successfully loaded: DefaultModule
info: Canister.IoC.Example[0]
      Adding singleton service: Canister.IoC.Example.RegisteredClass1 as Canister.IoC.Example.IRegisteredInterface
info: Canister.IoC.Example[0]
      Adding singleton service: Canister.IoC.Example.RegisteredClass2 as Canister.IoC.Example.IRegisteredInterface
Number of registered classes found: 2
Number of services found: 2
ExampleService1
ExampleService2
SimpleExampleClass

Select an example to run:
1. Basic module registration and resolution
2. Using UseLogger with Canister configuration
3. Using RegisterAll* without Canister configuration
Q. Quit
Enter your choice: 3
--- Example 3: Using RegisterAll* without Canister configuration ---
Number of registered classes found: 2
RegisteredClass1
RegisteredClass2

```
