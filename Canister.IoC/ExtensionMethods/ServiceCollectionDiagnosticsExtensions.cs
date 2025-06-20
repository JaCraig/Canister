using Microsoft.Extensions.Logging;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides diagnostic and debugging extension methods for IServiceCollection.
    /// </summary>
    public static class ServiceCollectionDiagnosticsExtensions
    {
        /// <summary>
        /// Returns a summary of all service registrations in the IServiceCollection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>A formatted string listing all service registrations.</returns>
        public static string GetRegistrationsSummary(this IServiceCollection services)
        {
            if (services == null)
                return string.Empty;
            var Sb = new StringBuilder();
            Sb.AppendLine("Service Registrations:");
            foreach (var Descriptor in services)
            {
                Sb.Append("- ServiceType: ").Append(Descriptor.ServiceType.FullName);
                if (Descriptor.ImplementationType != null)
                    Sb.Append(", ImplementationType: ").Append(Descriptor.ImplementationType.FullName);
                if (Descriptor.ImplementationInstance != null)
                    Sb.Append(", ImplementationInstance: ").Append(Descriptor.ImplementationInstance.GetType().FullName);
                if (Descriptor.ImplementationFactory != null)
                    Sb.Append(", ImplementationFactory: yes");
                Sb.Append(", Lifetime: ").Append(Descriptor.Lifetime);
                // .NET 8+ supports KeyedServiceDescriptor, but check for IsKeyedService property
                var IsKeyed = Descriptor.GetType().GetProperty("IsKeyedService")?.GetValue(Descriptor) as bool?;
                if (IsKeyed == true)
                {
                    var Key = Descriptor.GetType().GetProperty("ServiceKey")?.GetValue(Descriptor);
                    Sb.Append(", Key: ").Append(Key);
                }
                Sb.AppendLine();
            }
            return Sb.ToString();
        }

        /// <summary>
        /// Logs all service registrations in the IServiceCollection to the provided logger.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="logger">The logger to use.</param>
        /// <param name="level">The log level (default: Information).</param>
        public static void LogRegistrations(this IServiceCollection services, ILogger logger, LogLevel level = LogLevel.Information)
        {
            if (services == null || logger == null)
                return;
            logger.Log(level, services.GetRegistrationsSummary());
        }
    }
}