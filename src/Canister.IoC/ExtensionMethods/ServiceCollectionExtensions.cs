using Canister.Interfaces;
using Canister.IoC.Utils;
using Fast.Activator;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Service collection extension methods
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// The lock object
        /// </summary>
        private static readonly object AssemblyLockObject = new();

        /// <summary>
        /// The type lock object
        /// </summary>
        private static readonly object TypeLockObject = new();

        /// <summary>
        /// The assemblies
        /// </summary>
        private static Assembly[]? assemblies;

        /// <summary>
        /// The available types
        /// </summary>
        private static Type[]? availableTypes;

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <value>The assemblies.</value>
        private static Assembly[] Assemblies
        {
            get
            {
                if (assemblies is not null)
                    return assemblies;
                lock (AssemblyLockObject)
                {
                    if (assemblies is not null)
                        return assemblies;
                    assemblies = FindModules();
                    return assemblies;
                }
            }
        }

        /// <summary>
        /// Gets the available types.
        /// </summary>
        /// <value>The available types.</value>
        private static Type[] AvailableTypes
        {
            get
            {
                if (availableTypes is not null)
                    return availableTypes;
                lock (TypeLockObject)
                {
                    if (availableTypes is not null)
                        return availableTypes;
                    availableTypes = GetAvailableTypes();
                    return availableTypes;
                }
            }
        }

        /// <summary>
        /// Registers all objects of a certain type with the service collection as scoped.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection? AddAllScoped<T>(this IServiceCollection? serviceDescriptors)
        {
            return serviceDescriptors?.AddAllScoped(typeof(T));
        }

        /// <summary>
        /// Registers all objects of a certain type with the service collection as scoped.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="registerType">Type to register.</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection? AddAllScoped(this IServiceCollection? serviceDescriptors, Type registerType)
        {
            if (serviceDescriptors is null)
                return serviceDescriptors;
            foreach (Type? TempType in AvailableTypes.Where(type => registerType.IsAssignableFrom(type)))
            {
                serviceDescriptors.AddScoped(TempType, TempType);
                serviceDescriptors.AddScoped(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
                serviceDescriptors.AddScoped(registerType, registerType);
            return serviceDescriptors;
        }

        /// <summary>
        /// Registers all objects of a certain type with the service collection as a singleton.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection? AddAllSingleton<T>(this IServiceCollection? serviceDescriptors)
        {
            return serviceDescriptors?.AddAllSingleton(typeof(T));
        }

        /// <summary>
        /// Registers all objects of a certain type with the service collection as a singleton.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="registerType">Type to register.</param>
        /// <returns>
        /// The service collection
        /// </returns>
        public static IServiceCollection? AddAllSingleton(this IServiceCollection? serviceDescriptors, Type registerType)
        {
            if (serviceDescriptors is null)
                return serviceDescriptors;
            foreach (Type? TempType in AvailableTypes.Where(type => registerType.IsAssignableFrom(type)))
            {
                serviceDescriptors.AddSingleton(TempType, TempType);
                serviceDescriptors.AddSingleton(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
                serviceDescriptors.AddSingleton(registerType, registerType);
            return serviceDescriptors;
        }

        /// <summary>
        /// Registers all objects of a certain type with the service collection as a transient.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection? AddAllTransient<T>(this IServiceCollection? serviceDescriptors)
        {
            return serviceDescriptors?.AddAllTransient(typeof(T));
        }

        /// <summary>
        /// Registers all objects of a certain type with the service collection as a transient.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="registerType">Type to register.</param>
        /// <returns>
        /// The service collection
        /// </returns>
        public static IServiceCollection? AddAllTransient(this IServiceCollection? serviceDescriptors, Type registerType)
        {
            if (serviceDescriptors is null)
                return serviceDescriptors;
            foreach (Type? TempType in AvailableTypes.Where(type => registerType.IsAssignableFrom(type)))
            {
                serviceDescriptors.AddTransient(TempType, TempType);
                serviceDescriptors.AddTransient(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
                serviceDescriptors.AddTransient(registerType, registerType);
            return serviceDescriptors;
        }

        /// <summary>
        /// Finds the Canister modules and loads them into the service collection.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="configure">
        /// The configuration (if you wish to have more control and specify which assemblies to load
        /// the modules from).
        /// </param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddCanisterModules(this IServiceCollection? serviceDescriptors, Action<ICanisterConfiguration>? configure = default)
        {
            if (serviceDescriptors?.Any(x => x.ServiceType == typeof(CanisterRegisteredFlag)) != false)
                return serviceDescriptors;

            // Set up types
            var AssemblyConfiguration = new CanisterConfiguration();
            configure ??= LoadModules;
            configure(AssemblyConfiguration);
            assemblies = AssemblyConfiguration.Assemblies.ToArray();
            availableTypes = null;

            //Add assemblies and modules to the service collection
            serviceDescriptors.TryAddTransient<IEnumerable<Assembly>>(_ => AssemblyConfiguration.Assemblies);
            serviceDescriptors.AddAllTransient<IModule>();

            //Load modules to the service collection
            foreach (IModule ResolvedModule in GetAllOfType<IModule>().OrderBy(x => x.Order))
            {
                ResolvedModule.Load(serviceDescriptors);
            }
            serviceDescriptors.TryAddSingleton<CanisterRegisteredFlag>();

            //Clear info and return
            assemblies = null;
            availableTypes = null;
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
            IEnumerable<string> PathsFound = new List<string>
            {
                Path.GetDirectoryName(EntryAssembly.Location) ?? "",
                Path.GetDirectoryName(ExecutingAssembly.Location) ?? ""
            }.Distinct();

            foreach (var Path in PathsFound)
            {
                AssembliesFound.AddRange((IEnumerable<Assembly>)Directory.EnumerateFiles(Path, "*.dll", SearchOption.TopDirectoryOnly)
                    .Select(AssemblyPath =>
                    {
                        try
                        {
                            return Assembly.LoadFrom(AssemblyPath);
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    })
                    .Where(Assembly => Assembly is not null && !AssembliesFound.Contains(Assembly)));
            }

            return AssembliesFound.Distinct().ToArray();
        }

        /// <summary>
        /// Gets objects of a specific type and creates an instance.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <returns>The objects of a specific type.</returns>
        private static IEnumerable<TObject> GetAllOfType<TObject>() => AvailableTypes.Where(type => typeof(TObject).IsAssignableFrom(type)).Select(type => (TObject)FastActivator.CreateInstance(type));

        /// <summary>
        /// Gets the available types.
        /// </summary>
        /// <returns>The available types</returns>
        private static Type[] GetAvailableTypes()
        {
            return Assemblies.SelectMany(x =>
            {
                try
                {
                    return x.GetTypes();
                }
                catch (ReflectionTypeLoadException) { return Array.Empty<Type>(); }
            })
            .Where(x => x.IsClass
                && !x.IsAbstract
                && !x.ContainsGenericParameters)
            .ToArray();
        }

        /// <summary>
        /// Loads the default modules.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        private static void LoadModules(ICanisterConfiguration configuration) => configuration.AddAssembly(Assemblies);
    }
}