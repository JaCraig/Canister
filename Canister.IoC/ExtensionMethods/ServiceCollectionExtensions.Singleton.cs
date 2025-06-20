using Canister.IoC.Utils;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Service collection extension methods
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all objects of a certain type with the service collection as a singleton.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <returns>The service collection</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddAllSingleton<T>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.AddAllSingleton(typeof(T));

        /// <summary>
        /// Registers all objects of a certain type with the service collection as a singleton.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="registerType">Type to register.</param>
        /// <returns>The service collection</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddAllSingleton(this IServiceCollection? serviceDescriptors, Type registerType)
        {
            if (serviceDescriptors is null)
                return serviceDescriptors;
            var CanisterConfiguration = GetCanisterConfiguration(serviceDescriptors);
            foreach (Type? TempType in CanisterConfiguration.AvailableTypes.Where(registerType.IsAssignableFrom))
            {
                CanisterConfiguration.Log("Adding singleton service: {0} as {1}", TempType.FullName, registerType.FullName);
                _ = serviceDescriptors.AddSingleton(TempType, TempType);
                _ = serviceDescriptors.AddSingleton(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
                _ = serviceDescriptors.AddSingleton(registerType, registerType);
            return serviceDescriptors;
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedSingletonIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedSingleton<TService, TImplementation>(serviceKey);
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
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
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddSingletonIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddSingleton<TService, TImplementation>();
        }

        /// <summary>
        /// Attempts to add all implementations of the specified service type as singleton services
        /// to the service collection.
        /// </summary>
        /// <remarks>
        /// This method adds all implementations of the specified service type <typeparamref
        /// name="T"/> as singleton services to the provided <see cref="IServiceCollection"/>. If
        /// <paramref name="serviceDescriptors"/> is null, the method returns null without
        /// performing any operation.
        /// </remarks>
        /// <typeparam name="T">The type of the service to add as singleton.</typeparam>
        /// <param name="serviceDescriptors">
        /// The service collection to which the services will be added. Can be null.
        /// </param>
        /// <returns>
        /// The updated <see cref="IServiceCollection"/> if <paramref name="serviceDescriptors"/> is
        /// not null; otherwise, null.
        /// </returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? TryAddAllSingleton<T>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.TryAddAllSingleton(typeof(T));

        /// <summary>
        /// Attempts to add all available types that implement or inherit from the specified type as
        /// singleton services to the service collection.
        /// </summary>
        /// <remarks>
        /// This method scans the available types in the application's configuration and registers
        /// all types that are assignable to <paramref name="registerType"/> as singleton services.
        /// If <paramref name="registerType"/> is a generic type definition, it is also registered
        /// as a singleton service.
        /// </remarks>
        /// <param name="serviceDescriptors">
        /// The service collection to which the singleton services will be added. Can be <see langword="null"/>.
        /// </param>
        /// <param name="registerType">
        /// The type to register as a singleton service. This can be a concrete or generic type definition.
        /// </param>
        /// <returns>
        /// The updated <see cref="IServiceCollection"/> if <paramref name="serviceDescriptors"/> is
        /// not <see langword="null"/>; otherwise, <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? TryAddAllSingleton(this IServiceCollection? serviceDescriptors, Type registerType)
        {
            if (serviceDescriptors is null)
                return serviceDescriptors;
            var CanisterConfiguration = GetCanisterConfiguration(serviceDescriptors);
            foreach (Type? TempType in CanisterConfiguration.AvailableTypes.Where(registerType.IsAssignableFrom))
            {
                CanisterConfiguration.Log("Adding singleton service: {0} as {1}", TempType.FullName, registerType.FullName);
                serviceDescriptors.TryAddSingleton(TempType, TempType);
                serviceDescriptors.TryAddSingleton(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
                serviceDescriptors.TryAddSingleton(registerType, registerType);
            return serviceDescriptors;
        }
    }
}