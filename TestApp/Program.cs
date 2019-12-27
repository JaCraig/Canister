using Canister.IoC.Default;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace TestApp
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new CanisterServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                              .UseSerilog();
                });

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}