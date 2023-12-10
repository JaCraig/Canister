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
        /// Gets the assemblies.
        /// </summary>
        /// <value>The assemblies.</value>
        private static Assembly[] Assemblies
        {
            get
            {
                if (_Assemblies is not null)
                    return _Assemblies;
                lock (_AssemblyLockObject)
                {
                    if (_Assemblies is not null)
                        return _Assemblies;
                    _Assemblies = FindModules();
                    return _Assemblies;
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
                if (_AvailableTypes is not null)
                    return _AvailableTypes;
                lock (_TypeLockObject)
                {
                    if (_AvailableTypes is not null)
                        return _AvailableTypes;
                    _AvailableTypes = GetAvailableTypes();
                    return _AvailableTypes;
                }
            }
        }

        /// <summary>
        /// The lock object
        /// </summary>
        private static readonly object _AssemblyLockObject = new();

        /// <summary>
        /// The type lock object
        /// </summary>
        private static readonly object _TypeLockObject = new();

        /// <summary>
        /// The assemblies
        /// </summary>
        private static Assembly[]? _Assemblies;

        /// <summary>
        /// The available types
        /// </summary>
        private static Type[]? _AvailableTypes;

        /// <summary>
        /// Registers all objects of a certain type with the service collection as scoped.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection? AddAllScoped<T>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.AddAllScoped(typeof(T));

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
                _ = serviceDescriptors.AddScoped(TempType, TempType);
                _ = serviceDescriptors.AddScoped(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
                _ = serviceDescriptors.AddScoped(registerType, registerType);
            return serviceDescriptors;
        }

        /// <summary>
        /// Registers all objects of a certain type with the service collection as a singleton.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection? AddAllSingleton<T>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.AddAllSingleton(typeof(T));

        /// <summary>
        /// Registers all objects of a certain type with the service collection as a singleton.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="registerType">Type to register.</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection? AddAllSingleton(this IServiceCollection? serviceDescriptors, Type registerType)
        {
            if (serviceDescriptors is null)
                return serviceDescriptors;
            foreach (Type? TempType in AvailableTypes.Where(type => registerType.IsAssignableFrom(type)))
            {
                _ = serviceDescriptors.AddSingleton(TempType, TempType);
                _ = serviceDescriptors.AddSingleton(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
                _ = serviceDescriptors.AddSingleton(registerType, registerType);
            return serviceDescriptors;
        }

        /// <summary>
        /// Registers all objects of a certain type with the service collection as a transient.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection? AddAllTransient<T>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.AddAllTransient(typeof(T));

        /// <summary>
        /// Registers all objects of a certain type with the service collection as a transient.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="registerType">Type to register.</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection? AddAllTransient(this IServiceCollection? serviceDescriptors, Type registerType)
        {
            if (serviceDescriptors is null)
                return serviceDescriptors;
            foreach (Type? TempType in AvailableTypes.Where(type => registerType.IsAssignableFrom(type)))
            {
                _ = serviceDescriptors.AddTransient(TempType, TempType);
                _ = serviceDescriptors.AddTransient(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
                _ = serviceDescriptors.AddTransient(registerType, registerType);
            return serviceDescriptors;
        }

        /// <summary>
        /// Finds the Canister modules and loads them into the service collection.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="assemblies">Assemblies</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddCanisterModules(this IServiceCollection? serviceDescriptors, params Assembly[] assemblies) => serviceDescriptors?.AddCanisterModules(configure => configure.AddAssembly(assemblies));

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
            if (serviceDescriptors?.Exists<CanisterRegisteredFlag>() != false)
                return serviceDescriptors;

            // Set up types
            var AssemblyConfiguration = new CanisterConfiguration();
            configure ??= LoadModules;
            configure(AssemblyConfiguration);
            _Assemblies = AssemblyConfiguration.Assemblies.ToArray();
            _AvailableTypes = null;

            //Add assemblies and modules to the service collection
            serviceDescriptors.TryAddTransient<IEnumerable<Assembly>>(_ => AssemblyConfiguration.Assemblies);
            _ = serviceDescriptors.AddAllTransient<IModule>();

            //Load modules to the service collection
            foreach (IModule ResolvedModule in GetAllOfType<IModule>().OrderBy(x => x.Order))
            {
                ResolvedModule.Load(serviceDescriptors);
            }
            serviceDescriptors.TryAddSingleton<CanisterRegisteredFlag>();

            //Clear info and return
            _Assemblies = null;
            _AvailableTypes = null;
            return serviceDescriptors;
        }

        /// <summary>
        /// Adds a Scoped service of the type specified in TService with a factory specified in
        /// implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="serviceKey">Service key</param>
        /// <param name="implementationType">Implementation type</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedScopedIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Type implementationType)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedScoped(serviceType, serviceKey, implementationType);
        }

        /// <summary>
        /// Adds a Scoped service of the type specified in serviceType with an implementation
        /// factory specified in implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="serviceKey">Service key</param>
        /// <param name="implementationFactory">Implementation factory</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedScopedIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Func<IServiceProvider, object?, object> implementationFactory)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedScoped(serviceType, serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Scoped service of the type specified in serviceType with an implementation of the
        /// type specified in implementationType to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="serviceKey">Service key</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedScopedIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey) => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddKeyedScoped(serviceType, serviceKey);

        /// <summary>
        /// Adds a Scoped service of the type specified in TService with a factory specified in
        /// implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="serviceKey">Service key</param>
        /// <param name="implementationFactory">The implementation factory.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedScopedIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TService> implementationFactory)
            where TService : class
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedScoped(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Scoped service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="serviceKey">Service key</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedScopedIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddKeyedScoped<TService>(serviceKey);

        /// <summary>
        /// Adds a Scoped service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplementation">Implementation type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="serviceKey">Service key</param>
        /// <param name="implementationFactory">The implementation factory.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedScopedIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedScoped<TService, TImplementation>(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Scoped service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <typeparam name="TImplementation">Implementation type.</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="serviceKey">Service key</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedScopedIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedScoped<TService, TImplementation>(serviceKey);
        }

        /// <summary>
        /// Adds a Singleton service of the type specified in TService with a factory specified in
        /// implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="serviceKey">Service key</param>
        /// <param name="implementationType">Implementation type</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedSingletonIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Type implementationType)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedSingleton(serviceType, serviceKey, implementationType);
        }

        /// <summary>
        /// Adds a Singleton service of the type specified in serviceType with an implementation
        /// factory specified in implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="serviceKey">Service key</param>
        /// <param name="implementationFactory">Implementation factory</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedSingletonIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Func<IServiceProvider, object?, object> implementationFactory)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedSingleton(serviceType, serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Singleton service of the type specified in serviceType with an implementation of
        /// the type specified in implementationType to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="serviceKey">Service key</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedSingletonIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey) => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddKeyedSingleton(serviceType, serviceKey);

        /// <summary>
        /// Adds a Singleton service of the type specified in TService with a factory specified in
        /// implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="serviceKey">Service key</param>
        /// <param name="implementationFactory">The implementation factory.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedSingletonIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TService> implementationFactory)
            where TService : class
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedSingleton(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Singleton service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="serviceKey">Service key</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedSingletonIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddKeyedSingleton<TService>(serviceKey);

        /// <summary>
        /// Adds a Singleton service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplementation">Implementation type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="serviceKey">Service key</param>
        /// <param name="implementationFactory">The implementation factory.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedSingletonIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedSingleton<TService, TImplementation>(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Singleton service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <typeparam name="TImplementation">Implementation type.</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="serviceKey">Service key</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedSingletonIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedSingleton<TService, TImplementation>(serviceKey);
        }

        /// <summary>
        /// Adds a Transient service of the type specified in TService with a factory specified in
        /// implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="serviceKey">Service key</param>
        /// <param name="implementationType">Implementation type</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Type implementationType)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedTransient(serviceType, serviceKey, implementationType);
        }

        /// <summary>
        /// Adds a Transient service of the type specified in serviceType with an implementation
        /// factory specified in implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="serviceKey">Service key</param>
        /// <param name="implementationFactory">Implementation factory</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Func<IServiceProvider, object?, object> implementationFactory)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedTransient(serviceType, serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Transient service of the type specified in serviceType with an implementation of
        /// the type specified in implementationType to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="serviceKey">Service key</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey) => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddKeyedTransient(serviceType, serviceKey);

        /// <summary>
        /// Adds a Transient service of the type specified in TService with a factory specified in
        /// implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="serviceKey">Service key</param>
        /// <param name="implementationFactory">The implementation factory.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedTransientIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TService> implementationFactory)
            where TService : class
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedTransient(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Transient service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="serviceKey">Service key</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedTransientIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddKeyedTransient<TService>(serviceKey);

        /// <summary>
        /// Adds a Transient service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplementation">Implementation type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="serviceKey">Service key</param>
        /// <param name="implementationFactory">The implementation factory.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedTransientIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedTransient<TService, TImplementation>(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Transient service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <typeparam name="TImplementation">Implementation type.</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="serviceKey">Service key</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddKeyedTransientIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedTransient<TService, TImplementation>(serviceKey);
        }

        /// <summary>
        /// Adds a Scoped service of the type specified in TService with a factory specified in
        /// implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="implementationType">Implementation type</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddScopedIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Type implementationType)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddScoped(serviceType, implementationType);
        }

        /// <summary>
        /// Adds a Scoped service of the type specified in serviceType with an implementation
        /// factory specified in implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="implementationFactory">Implementation factory</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddScopedIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddScoped(serviceType, implementationFactory);
        }

        /// <summary>
        /// Adds a Scoped service of the type specified in serviceType with an implementation of the
        /// type specified in implementationType to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddScopedIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType) => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddScoped(serviceType);

        /// <summary>
        /// Adds a Scoped service of the type specified in TService with a factory specified in
        /// implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="implementationFactory">The implementation factory.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddScopedIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddScoped(implementationFactory);
        }

        /// <summary>
        /// Adds a Scoped service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddScopedIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddScoped<TService>();

        /// <summary>
        /// Adds a Scoped service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplementation">Implementation type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="implementationFactory">The implementation factory.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddScopedIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddScoped<TService, TImplementation>(implementationFactory);
        }

        /// <summary>
        /// Adds a Scoped service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <typeparam name="TImplementation">Implementation type.</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddScopedIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddScoped<TService, TImplementation>();
        }

        /// <summary>
        /// Adds a Singleton service of the type specified in TService with a factory specified in
        /// implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="implementationType">Implementation type</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddSingletonIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Type implementationType)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddSingleton(serviceType, implementationType);
        }

        /// <summary>
        /// Adds a Singleton service of the type specified in serviceType with an implementation
        /// factory specified in implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="implementationFactory">Implementation factory</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddSingletonIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddSingleton(serviceType, implementationFactory);
        }

        /// <summary>
        /// Adds a Singleton service of the type specified in serviceType with an implementation of
        /// the type specified in implementationType to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddSingletonIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddSingleton(serviceType);
        }

        /// <summary>
        /// Adds a Singleton service of the type specified in TService with a factory specified in
        /// implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="implementationFactory">The implementation factory.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddSingletonIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddSingleton(implementationFactory);
        }

        /// <summary>
        /// Adds a Singleton service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddSingletonIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddSingleton<TService>();
        }

        /// <summary>
        /// Adds a Singleton service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplementation">Implementation type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="implementationFactory">The implementation factory.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddSingletonIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddSingleton<TService, TImplementation>(implementationFactory);
        }

        /// <summary>
        /// Adds a Singleton service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <typeparam name="TImplementation">Implementation type.</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddSingletonIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddSingleton<TService, TImplementation>();
        }

        /// <summary>
        /// Adds a transient service of the type specified in TService with a factory specified in
        /// implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="implementationType">Implementation type</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Type implementationType)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient(serviceType, implementationType);
        }

        /// <summary>
        /// Adds a transient service of the type specified in serviceType with an implementation
        /// factory specified in implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <param name="implementationFactory">Implementation factory</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient(serviceType, implementationFactory);
        }

        /// <summary>
        /// Adds a transient service of the type specified in serviceType with an implementation of
        /// the type specified in implementationType to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        /// <param name="predicate">Predicate</param>
        /// <param name="serviceType">Service type</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient(serviceType);
        }

        /// <summary>
        /// Adds a transient service of the type specified in TService with a factory specified in
        /// implementationFactory to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="implementationFactory">The implementation factory.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddTransientIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient(implementationFactory);
        }

        /// <summary>
        /// Adds a transient service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddTransientIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient<TService>();
        }

        /// <summary>
        /// Adds a transient service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplementation">Implementation type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="implementationFactory">The implementation factory.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddTransientIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient<TService, TImplementation>(implementationFactory);
        }

        /// <summary>
        /// Adds a transient service of the type specified in TService with an implementation type
        /// specified in TImplementation to the specified
        /// Microsoft.Extensions.DependencyInjection.IServiceCollection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <typeparam name="TImplementation">Implementation type.</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection? AddTransientIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient<TService, TImplementation>();
        }

        /// <summary>
        /// Determines if the type exists in the service collection.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <returns>True if it does, false otherwise.</returns>
        public static bool Exists<TService>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.Exists(typeof(TService)) ?? false;

        /// <summary>
        /// Determines if the type exists in the service collection.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="serviceType">The type of the service.</param>
        /// <returns>True if it does, false otherwise.</returns>
        public static bool Exists(this IServiceCollection? serviceDescriptors, Type serviceType) => serviceDescriptors?.Any(service => service.ServiceType == serviceType) ?? false;

        /// <summary>
        /// Determines if the type exists in the service collection.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="serviceKey">The service key.</param>
        /// <returns>True if it does, false otherwise.</returns>
        public static bool Exists(this IServiceCollection? serviceDescriptors, Type serviceType, object? serviceKey) => serviceDescriptors?.Any(service => service.IsKeyedService && service.ServiceType == serviceType && service.ServiceKey == serviceKey) ?? false;

        /// <summary>
        /// Determines if the type exists in the service collection.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="serviceKey">The service key.</param>
        /// <returns>True if it does, false otherwise.</returns>
        public static bool Exists<TService>(this IServiceCollection? serviceDescriptors, object? serviceKey) => serviceDescriptors?.Exists(typeof(TService), serviceKey) ?? false;

        /// <summary>
        /// Determines if the type exists in the service collection.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <returns>True if it does, false otherwise.</returns>
        public static bool Exists<TService, TImplementation>(this IServiceCollection? serviceDescriptors)
            where TService : class
            where TImplementation : class, TService => serviceDescriptors?.Exists(typeof(TService), typeof(TImplementation)) ?? false;

        /// <summary>
        /// Determines if the type exists in the service collection.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="implementationType">The type of the implementation.</param>
        /// <returns>True if it does, false otherwise.</returns>
        public static bool Exists(this IServiceCollection? serviceDescriptors, Type serviceType, Type implementationType) => serviceDescriptors?.Any(service => service.ServiceType == serviceType && service.ImplementationType == implementationType) ?? false;

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
                    .Select(assemblyPath =>
                    {
                        try
                        {
                            return Assembly.LoadFrom(assemblyPath);
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    })
                    .Where(assembly => assembly is not null && !AssembliesFound.Contains(assembly)));
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