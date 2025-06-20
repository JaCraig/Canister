using Canister.Interfaces;
using Canister.IoC.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Canister.IoC.Example
{
    // Interfaces and classes used in the examples
    internal interface IMyService
    {
        string Name { get; }
    }

    [RegisterAll(ServiceLifetime.Singleton)]
    internal interface IRegisteredInterface
    { }

    // Example 1: Basic module registration and resolution
    internal static class Example1
    {
        public static void Run()
        {
            Console.WriteLine("--- Example 1: Basic module registration and resolution ---");
            var services = new ServiceCollection();
            services.AddCanisterModules();
            services.AddAllSingleton<IMyService>();
            var provider = services.BuildServiceProvider();

            var registered = provider.GetServices<IRegisteredInterface>();
            Console.WriteLine($"Number of registered classes found: {registered.Count()}");

            var serviceClasses = provider.GetServices<IMyService>();
            Console.WriteLine($"Number of services found: {serviceClasses.Count()}");
            foreach (var svc in serviceClasses)
                Console.WriteLine(svc.Name);

            var simple = provider.GetService<SimpleExampleClass>();
            Console.WriteLine(simple?.Name);
        }
    }

    // Example 2: Using UseLogger with Canister configuration
    internal static class Example2
    {
        public static void Run()
        {
            Console.WriteLine("--- Example 2: Using UseLogger with Canister configuration ---");
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            var logger = loggerFactory.CreateLogger("Canister.IoC.Example");

            var services = new ServiceCollection();
            services.AddCanisterModules(configure =>
                configure.UseLogger(logger, LogLevel.Information));
            services.AddAllSingleton<IMyService>();
            var provider = services.BuildServiceProvider();

            var registered = provider.GetServices<IRegisteredInterface>();
            Console.WriteLine($"Number of registered classes found: {registered.Count()}");

            var serviceClasses = provider.GetServices<IMyService>();
            Console.WriteLine($"Number of services found: {serviceClasses.Count()}");
            foreach (var svc in serviceClasses)
                Console.WriteLine(svc.Name);

            var simple = provider.GetService<SimpleExampleClass>();
            Console.WriteLine(simple?.Name);
        }
    }

    // Example 3: Using RegisterAll* without Canister configuration
    internal static class Example3
    {
        public static void Run()
        {
            Console.WriteLine("--- Example 3: Using RegisterAll* without Canister configuration ---");
            var services = new ServiceCollection();
            services.AddAllScoped<IRegisteredInterface>();
            var provider = services.BuildServiceProvider();
            var registered = provider.GetServices<IRegisteredInterface>();
            Console.WriteLine($"Number of registered classes found: {registered.Count()}");
            foreach (var reg in registered)
                Console.WriteLine(reg.GetType().Name);
        }
    }

    internal static class Program
    {
        private static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Select an example to run:");
                Console.WriteLine("1. Basic module registration and resolution");
                Console.WriteLine("2. Using UseLogger with Canister configuration");
                Console.WriteLine("3. Using RegisterAll* without Canister configuration");
                Console.WriteLine("Q. Quit");
                Console.Write("Enter your choice: ");
                var input = Console.ReadLine();
                if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                    break;
                switch (input)
                {
                    case "1":
                        Example1.Run();
                        break;

                    case "2":
                        Example2.Run();
                        break;

                    case "3":
                        Example3.Run();
                        break;

                    default:
                        Console.WriteLine("Invalid selection.\n");
                        break;
                }
                Console.WriteLine();
            }
        }
    }

    internal class ExampleService1 : IMyService
    {
        public string Name => "ExampleService1";
    }

    internal class ExampleService2 : IMyService
    {
        public string Name => "ExampleService2";
    }

    internal class MyModule : IModule
    {
        public int Order { get; }

        public void Load(IServiceCollection serviceDescriptors) => serviceDescriptors.AddTransient<SimpleExampleClass>();
    }

    internal class RegisteredClass1 : IRegisteredInterface
    { }

    internal class RegisteredClass2 : IRegisteredInterface
    { }

    internal class SimpleExampleClass
    { public string Name => "SimpleExampleClass"; }
}