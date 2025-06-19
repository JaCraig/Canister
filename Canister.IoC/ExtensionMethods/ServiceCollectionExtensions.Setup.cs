using Canister.Interfaces;
using Canister.IoC.Attributes;
using Canister.IoC.Utils;
using Fast.Activator;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/> to register Canister modules
    /// and types.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Stores the Canister configuration for each <see cref="IServiceCollection"/> instance.
        /// </summary>
        private static readonly Dictionary<IServiceCollection, CanisterConfiguration> _CanisterConfiguration = [];

        /// <summary>
        /// Lock object used to synchronize access to the Canister configuration dictionary.
        /// </summary>
        private static readonly object _CanisterLockObject = new();

        /// <summary>
        /// Finds the Canister modules and loads them into the service collection using the
        /// specified assemblies.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="assemblies">Assemblies to scan for modules and registrations.</param>
        /// <returns>The service collection with Canister modules loaded.</returns>
        public static IServiceCollection? AddCanisterModules(this IServiceCollection? serviceDescriptors, params Assembly[] assemblies) => serviceDescriptors?.AddCanisterModules(configure => configure.AddAssembly(assemblies));

        /// <summary>
        /// Finds the Canister modules and loads them into the service collection using a
        /// configuration action.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="configure">
        /// The configuration action to specify which assemblies to load the modules from.
        /// </param>
        /// <returns>The service collection with Canister modules loaded.</returns>
        public static IServiceCollection? AddCanisterModules(this IServiceCollection? serviceDescriptors, Action<ICanisterConfiguration>? configure = default)
        {
            if (serviceDescriptors?.Exists<CanisterRegisteredFlag>() != false)
                return serviceDescriptors;

            // Set up types
            configure ??= (config) => config.AddDefaultAssemblies();
            var CanisterConfiguration = GetCanisterConfiguration(serviceDescriptors);
            configure(CanisterConfiguration);

            // Add assemblies and modules to the service collection
            serviceDescriptors.TryAddTransient<IEnumerable<Assembly>>(_ => CanisterConfiguration.Assemblies);
            _ = serviceDescriptors.AddAllTransient<IModule>();

            // Load modules to the service collection
            foreach (IModule ResolvedModule in CanisterConfiguration
                .AvailableTypes
                .Where(type => typeof(IModule).IsAssignableFrom(type))
                .Select(type => (IModule)FastActivator.CreateInstance(type))
                .OrderBy(x => x.Order))
            {
                ResolvedModule.Load(serviceDescriptors);
            }

            // Load types with the RegisterAttribute to the service collection
            foreach (Type RegisteredType in CanisterConfiguration
                .AvailableTypes
                .Where(type => type.GetCustomAttributes<RegisterAttribute>().Any())
                .Concat(CanisterConfiguration
                    .AvailableInterfaces
                    .Where(type => type.GetCustomAttributes<RegisterAllAttribute>().Any())))
            {
                RegisterClasses(serviceDescriptors, RegisteredType);
                RegisterInterfaces(serviceDescriptors, RegisteredType);
            }

            serviceDescriptors.TryAddSingleton<CanisterRegisteredFlag>();
            _CanisterConfiguration.Remove(serviceDescriptors);

            // Clear info and return
            return serviceDescriptors;
        }

        /// <summary>
        /// Gets the <see cref="CanisterConfiguration"/> for the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The <see cref="CanisterConfiguration"/> instance.</returns>
        private static CanisterConfiguration GetCanisterConfiguration(IServiceCollection serviceCollection)
        {
            if (_CanisterConfiguration.TryGetValue(serviceCollection, out CanisterConfiguration? CanisterConfiguration))
                return CanisterConfiguration;
            lock (_CanisterLockObject)
            {
                if (_CanisterConfiguration.TryGetValue(serviceCollection, out CanisterConfiguration))
                    return CanisterConfiguration;
                CanisterConfiguration = new CanisterConfiguration();
                _CanisterConfiguration[serviceCollection] = CanisterConfiguration;
                return CanisterConfiguration;
            }
        }

        /// <summary>
        /// Registers the classes decorated with <see cref="RegisterAttribute"/> in the service collection.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection.</param>
        /// <param name="registeredType">The type to register.</param>
        private static void RegisterClasses(IServiceCollection? serviceDescriptors, Type registeredType)
        {
            if (serviceDescriptors is null)
                return;
            IEnumerable<Type> TypeChain = registeredType.GetInterfaces().Concat([registeredType]);
            foreach (RegisterAttribute RegisterAttribute in registeredType.GetCustomAttributes<RegisterAttribute>())
            {
                foreach (Type? ServiceType in TypeChain)
                {
                    switch (RegisterAttribute.Lifetime)
                    {
                        case ServiceLifetime.Scoped:
                            _ = RegisterAttribute.ServiceKey is null
                                ? serviceDescriptors.AddScoped(ServiceType, registeredType)
                                : serviceDescriptors.AddKeyedScoped(ServiceType, RegisterAttribute.ServiceKey, registeredType);
                            break;

                        case ServiceLifetime.Singleton:
                            _ = RegisterAttribute.ServiceKey is null
                                ? serviceDescriptors.AddSingleton(ServiceType, registeredType)
                                : serviceDescriptors.AddKeyedSingleton(ServiceType, RegisterAttribute.ServiceKey, registeredType);
                            break;

                        case ServiceLifetime.Transient:
                            _ = RegisterAttribute.ServiceKey is null
                                ? serviceDescriptors.AddTransient(ServiceType, registeredType)
                                : serviceDescriptors.AddKeyedTransient(ServiceType, RegisterAttribute.ServiceKey, registeredType);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Registers the interfaces decorated with <see cref="RegisterAllAttribute"/> in the
        /// service collection.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection.</param>
        /// <param name="registeredType">The interface type to register.</param>
        private static void RegisterInterfaces(IServiceCollection? serviceDescriptors, Type registeredType)
        {
            if (serviceDescriptors is null)
                return;
            foreach (RegisterAllAttribute RegisterAllAttribute in registeredType.GetCustomAttributes<RegisterAllAttribute>())
            {
                switch (RegisterAllAttribute.Lifetime)
                {
                    case ServiceLifetime.Scoped:
                        _ = serviceDescriptors.AddAllScoped(registeredType);
                        break;

                    case ServiceLifetime.Singleton:
                        _ = serviceDescriptors.AddAllSingleton(registeredType);
                        break;

                    case ServiceLifetime.Transient:
                        _ = serviceDescriptors.AddAllTransient(registeredType);
                        break;
                }
            }
        }
    }
}