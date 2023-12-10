using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Canister.IoC.Example
{
    /// <summary>
    /// This is an example service interface that will be loaded into the service collection
    /// </summary>
    internal interface IMyService
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
    }

    /// <summary>
    /// Example of how to use Canister
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            // Basic service collection
            ServiceProvider? ServiceProvider = new ServiceCollection()
                                    // Add the canister modules
                                    .AddCanisterModules()
                                    // Add all classes that implement IMyService as singletons
                                    .AddAllSingleton<IMyService>()
                                    // Build the service provider
                                    ?.BuildServiceProvider();

            // Get all the services that implement IMyService
            IEnumerable<IMyService> ServiceClasses = ServiceProvider?.GetServices<IMyService>() ?? Array.Empty<IMyService>();
            // Write out the number of services found (should be 2)
            Console.WriteLine("Number of services found: {0}", ServiceClasses.Count());
            // Write out the names of the services found (should be ExampleService1 and ExampleService2)
            foreach (IMyService ServiceClass in ServiceClasses)
            {
                Console.WriteLine(ServiceClass.Name);
            }
            // Write out the name of the simple example class (should be SimpleExampleClass)
            SimpleExampleClass? SimpleExampleClass = ServiceProvider?.GetService<SimpleExampleClass>();
            Console.WriteLine(SimpleExampleClass?.Name);
        }
    }

    /// <summary>
    /// Example service 1
    /// </summary>
    /// <seealso cref="IMyService"/>
    internal class ExampleService1 : IMyService
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => "ExampleService1";
    }

    /// <summary>
    /// Example service 2
    /// </summary>
    /// <seealso cref="IMyService"/>
    internal class ExampleService2 : IMyService
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => "ExampleService2";
    }

    /// <summary>
    /// This is a module that will be called and loaded into the service collection
    /// </summary>
    /// <seealso cref="IModule"/>
    internal class MyModule : IModule
    {
        /// <summary>
        /// Order to run this in
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// Loads the module using the service collection.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        public void Load(IServiceCollection serviceDescriptors) => serviceDescriptors.AddTransient<SimpleExampleClass>();
    }

    /// <summary>
    /// This is a simple example class
    /// </summary>
    internal class SimpleExampleClass
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => "SimpleExampleClass";
    }
}