using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace TestApp.Modules
{
    /// <summary>
    /// Canister module
    /// </summary>
    /// <seealso cref="IModule"/>
    public class CanisterModule : IModule
    {
        /// <summary>
        /// Order to run this in
        /// </summary>
        public int Order => 1;

        /// <summary>
        /// Loads the module using the service collection.
        /// </summary>
        /// <param name="bootstrapper"></param>
        public void Load(IServiceCollection bootstrapper)
        {
            var RootPath = ".";
            try
            {
                IHostEnvironment HostingEnvironment = bootstrapper.BuildServiceProvider().GetService<IHostEnvironment>();
                RootPath = HostingEnvironment.ContentRootPath;
            }
            catch { }
            var Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var AssemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            Log.Logger = new LoggerConfiguration()
                                            .MinimumLevel
#if RELEASE
                                            .Information()
#else
                                            .Debug()
#endif
                                            .Enrich.FromLogContext()
                                            .WriteTo
                                                .File(
                                                    RootPath + "/Logs/log-.txt",
                                                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{UserName}] {Message}{NewLine}{Exception}",
                                                    rollingInterval: RollingInterval.Day)
                                            .CreateLogger();
            bootstrapper.TryAddTransient(_ => Log.Logger);
        }
    }
}