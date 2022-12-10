using Canister.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace TestApp.Modules
{
    /// <summary>
    /// Configuration
    /// </summary>
    /// <seealso cref="IModule"/>
    public class ConfigurationModule : IModule
    {
        /// <summary>
        /// Order to run this in
        /// </summary>
        public int Order { get; } = 1;

        /// <summary>
        /// Loads the module using the bootstrapper
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper.</param>
        public void Load(IServiceCollection bootstrapper)
        {
            if (bootstrapper == null)
                return;
            IConfigurationRoot Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            bootstrapper.AddTransient<IConfiguration>(_ => Configuration);
            bootstrapper.AddTransient(_ => Configuration);
        }
    }
}