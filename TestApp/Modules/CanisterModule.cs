using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
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

        public void Load(IBootstrapper bootstrapper)
        {
            string RootPath = ".";
            try
            {
                var HostingEnvironment = bootstrapper.Resolve<IHostEnvironment>();
                RootPath = HostingEnvironment.ContentRootPath;
            }
            catch { }
            var Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var AssemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            var IndexName = "errorlog-" + AssemblyName.ToLower() + "-" + Environment.ToLower() + "-{0:yyyy.MM.dd}";
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
                                                    RootPath + "/Logs/log-{Date}.txt",
                                                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{UserName}] {Message}{NewLine}{Exception}",
                                                    rollingInterval: RollingInterval.Day)
                                            .CreateLogger();
            bootstrapper.Register(Log.Logger, ServiceLifetime.Singleton);
        }
    }
}