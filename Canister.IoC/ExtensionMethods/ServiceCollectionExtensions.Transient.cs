using Canister.IoC.Utils;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/> to register transient and
    /// keyed transient services, including conditional registration based on predicates.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all objects of a certain type with the service collection as a transient.
        /// </summary>
        /// <typeparam name="T">The object type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the services to.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddAllTransient<T>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.AddAllTransient(typeof(T));

        /// <summary>
        /// Registers all objects of a certain type with the service collection as a transient.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the services to.</param>
        /// <param name="registerType">The type to register.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddAllTransient(this IServiceCollection? serviceDescriptors, Type registerType)
        {
            if (serviceDescriptors is null)
                return serviceDescriptors;
            var CanisterConfiguration = GetCanisterConfiguration(serviceDescriptors);
            foreach (Type? TempType in CanisterConfiguration.AvailableTypes.Where(registerType.IsAssignableFrom))
            {
                CanisterConfiguration.Log("Adding transient service: {0} as {1}", TempType.FullName, registerType.FullName);
                _ = serviceDescriptors.AddTransient(TempType, TempType);
                _ = serviceDescriptors.AddTransient(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
                _ = serviceDescriptors.AddTransient(registerType, registerType);
            return serviceDescriptors;
        }

        /// <summary>
        /// Adds a transient service of the specified type with a keyed implementation type to the
        /// service collection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="serviceKey">The key for the service.</param>
        /// <param name="implementationType">The implementation type to use.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Type implementationType)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedTransient(serviceType, serviceKey, implementationType);
        }

        /// <summary>
        /// Adds a transient service of the specified type with a keyed implementation factory to
        /// the service collection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="serviceKey">The key for the service.</param>
        /// <param name="implementationFactory">A factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Func<IServiceProvider, object?, object> implementationFactory)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedTransient(serviceType, serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a transient service of the specified type with a keyed implementation to the
        /// service collection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="serviceKey">The key for the service.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey) => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddKeyedTransient(serviceType, serviceKey);

        /// <summary>
        /// Adds a transient service of the specified type with a keyed implementation factory to
        /// the service collection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service.</param>
        /// <param name="implementationFactory">A factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedTransientIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TService> implementationFactory)
            where TService : class
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedTransient(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a transient service of the specified type with a keyed implementation to the
        /// service collection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedTransientIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddKeyedTransient<TService>(serviceKey);

        /// <summary>
        /// Adds a transient service of the specified type with a keyed implementation factory to
        /// the service collection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service.</param>
        /// <param name="implementationFactory">A factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedTransientIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedTransient<TService, TImplementation>(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a transient service of the specified type with a keyed implementation to the
        /// service collection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedTransientIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedTransient<TService, TImplementation>(serviceKey);
        }

        /// <summary>
        /// Adds a transient service of the specified type with an implementation type to the
        /// service collection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementationType">The implementation type to use.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Type implementationType)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient(serviceType, implementationType);
        }

        /// <summary>
        /// Adds a transient service of the specified type with an implementation factory to the
        /// service collection if the predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementationFactory">A factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient(serviceType, implementationFactory);
        }

        /// <summary>
        /// Adds a transient service of the specified type to the service collection if the
        /// predicate is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient(serviceType);
        }

        /// <summary>
        /// Adds a transient service of the specified type with an implementation factory to the
        /// service collection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="implementationFactory">A factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddTransientIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient(implementationFactory);
        }

        /// <summary>
        /// Adds a transient service of the specified type to the service collection if the
        /// predicate is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddTransientIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient<TService>();
        }

        /// <summary>
        /// Adds a transient service of the specified type with an implementation factory to the
        /// service collection if the predicate is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="implementationFactory">A factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddTransientIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient<TService, TImplementation>(implementationFactory);
        }

        /// <summary>
        /// Adds a transient service of the specified type to the service collection if the
        /// predicate is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddTransientIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddTransient<TService, TImplementation>();
        }

        /// <summary>
        /// Attempts to add all implementations of the specified service type as transient services
        /// to the service collection.
        /// </summary>
        /// <remarks>
        /// This method registers all implementations of the specified service type <typeparamref
        /// name="T"/> as transient services. If <paramref name="serviceDescriptors"/> is <see
        /// langword="null"/>, no services are added, and the method returns <see langword="null"/>.
        /// </remarks>
        /// <typeparam name="T">The service type to register implementations for.</typeparam>
        /// <param name="serviceDescriptors">
        /// The service collection to which the services will be added. Can be <see langword="null"/>.
        /// </param>
        /// <returns>
        /// The updated <see cref="IServiceCollection"/> if <paramref name="serviceDescriptors"/> is
        /// not <see langword="null"/>; otherwise, <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? TryAddAllTransient<T>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.TryAddAllTransient(typeof(T));

        /// <summary>
        /// Attempts to add all available types as transient services to the specified service collection.
        /// </summary>
        /// <remarks>
        /// This method scans the available types in the application's configuration and registers
        /// all types that are assignable to <paramref name="registerType"/> as transient services.
        /// If <paramref name="registerType"/> is a generic type definition, it will also be
        /// registered as itself.
        /// </remarks>
        /// <param name="serviceDescriptors">
        /// The service collection to which the services will be added. Can be <see langword="null"/>.
        /// </param>
        /// <param name="registerType">
        /// The type to register as a transient service. If the type is a generic type definition,
        /// it will be registered as itself.
        /// </param>
        /// <returns>
        /// The updated <see cref="IServiceCollection"/> instance, or <see langword="null"/> if
        /// <paramref name="serviceDescriptors"/> is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? TryAddAllTransient(this IServiceCollection? serviceDescriptors, Type registerType)
        {
            if (serviceDescriptors is null)
                return serviceDescriptors;
            var CanisterConfiguration = GetCanisterConfiguration(serviceDescriptors);
            foreach (Type? TempType in CanisterConfiguration.AvailableTypes.Where(registerType.IsAssignableFrom))
            {
                CanisterConfiguration.Log("Adding transient service: {0} as {1}", TempType.FullName, registerType.FullName);
                serviceDescriptors.TryAddTransient(TempType, TempType);
                serviceDescriptors.TryAddTransient(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
                serviceDescriptors.TryAddTransient(registerType, registerType);
            return serviceDescriptors;
        }
    }
}