using Canister.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Service collection extension methods
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the canister modules.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="configure">The configure.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddCanisterModules(this IServiceCollection serviceDescriptors, Action<ICanisterConfiguration>? configure = default)
        {
            var Bootstrapper = Canister.Builder.CreateContainer(serviceDescriptors);
            configure ??= LoadModules;
            configure(Bootstrapper);
            Bootstrapper.Build();
            return serviceDescriptors;
        }

        /// <summary>
        /// Finds the modules.
        /// </summary>
        /// <returns>The assemblies that contain modules.</returns>
        private static Assembly[] FindModules()
        {
            var EntryAssembly = Assembly.GetEntryAssembly();
            if (EntryAssembly is null)
                return Array.Empty<Assembly>();
            var ExecutingAssembly = Assembly.GetExecutingAssembly();
            if (ExecutingAssembly is null)
                return Array.Empty<Assembly>();
            var AssembliesFound = new List<Assembly>
            {
                EntryAssembly,
                ExecutingAssembly
            };
            var PathsFound = new List<string>
            {
                EntryAssembly.Location,
                ExecutingAssembly.Location
            };
            foreach (var Path in PathsFound)
            {
                foreach (var TempAssembly in new FileInfo(Path).Directory.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        var LoadedTempAssembly = Assembly.Load(AssemblyName.GetAssemblyName(TempAssembly.FullName));
                        if (!AssembliesFound.Contains(LoadedTempAssembly))
                            AssembliesFound.Add(LoadedTempAssembly);
                    }
                    catch { }
                }
            }
            return AssembliesFound.ToArray();
        }

        /// <summary>
        /// Loads the default modules.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        private static void LoadModules(ICanisterConfiguration configuration)
        {
            configuration.AddAssembly(FindModules());
        }
    }
}