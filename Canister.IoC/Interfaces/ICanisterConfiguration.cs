using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection;

namespace Canister.Interfaces
{
    /// <summary>
    /// Defines the contract for configuring Canister by specifying which assemblies to include for
    /// dependency injection and service registration.
    /// </summary>
    public interface ICanisterConfiguration
    {
        /// <summary>
        /// Adds one or more assemblies to the Canister configuration for scanning and registration.
        /// </summary>
        /// <param name="assemblies">An array of <see cref="Assembly"/> instances to add.</param>
        /// <returns>The current <see cref="ICanisterConfiguration"/> instance for chaining.</returns>
        ICanisterConfiguration AddAssembly(params Assembly?[]? assemblies);

        /// <summary>
        /// Adds a collection of assemblies to the Canister configuration for scanning and registration.
        /// </summary>
        /// <param name="assemblies">
        /// An <see cref="IEnumerable{Assembly}"/> containing assemblies to add.
        /// </param>
        /// <returns>The current <see cref="ICanisterConfiguration"/> instance for chaining.</returns>
        ICanisterConfiguration AddAssembly(IEnumerable<Assembly?>? assemblies);

        /// <summary>
        /// Adds the default assemblies, typically including the entry assembly and its referenced
        /// assemblies, to the Canister configuration for scanning and registration.
        /// </summary>
        /// <returns>The current <see cref="ICanisterConfiguration"/> instance for chaining.</returns>
        ICanisterConfiguration AddDefaultAssemblies();

        /// <summary>
        /// Sets the logger and log level for diagnostics and logging in a fluent manner.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="logLevel">The minimum log level (default: Information).</param>
        /// <returns>The current <see cref="ICanisterConfiguration"/> instance for chaining.</returns>
        ICanisterConfiguration UseLogger(ILogger? logger, LogLevel logLevel = LogLevel.Information);
    }
}