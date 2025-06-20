using Canister.IoC.Utils;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/> to register services with
    /// Singleton lifetimes, including conditional and keyed registrations.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all objects of a certain type with the service collection as Singleton.
        /// </summary>
        /// <typeparam name="T">The object type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the services to.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddAllSingleton<T>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.AddAllSingleton(typeof(T));

        /// <summary>
        /// Registers all objects of a certain type with the service collection as Singleton.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the services to.</param>
        /// <param name="registerType">The type to register.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddAllSingleton(this IServiceCollection? serviceDescriptors, Type registerType)
        {
            if (serviceDescriptors is null)
                return serviceDescriptors;
            var CanisterConfiguration = GetCanisterConfiguration(serviceDescriptors);
            foreach (Type? TempType in CanisterConfiguration.AvailableTypes.Where(registerType.IsAssignableFrom))
            {
                CanisterConfiguration.Log("Adding singleton service: {0} as {1}", TempType.FullName, TempType.FullName);
                _ = serviceDescriptors.AddSingleton(TempType, TempType);
                CanisterConfiguration.Log("Adding singleton service: {0} as {1}", TempType.FullName, registerType.FullName);
                _ = serviceDescriptors.AddSingleton(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
            {
                CanisterConfiguration.Log("Adding singleton service: {0} as {1}", registerType.FullName, registerType.FullName);
                _ = serviceDescriptors.AddSingleton(registerType, registerType);
            }
            return serviceDescriptors;
        }

        /// <summary>
        /// Adds a Singleton service with a specified key to the service collection if the given
        /// predicate evaluates to <see langword="true"/>.
        /// </summary>
        /// <param name="serviceDescriptors">
        /// The <see cref="IServiceCollection"/> to which the service will be added.
        /// </param>
        /// <param name="predicate">
        /// A function that determines whether the service should be added. The service is added
        /// only if this function returns <see langword="true"/>.
        /// </param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="serviceKey">
        /// An optional key used to distinguish this service registration from others of the same type.
        /// </param>
        /// <param name="implementationType">The type that implements the service.</param>
        /// <returns>
        /// The updated <see cref="IServiceCollection"/> if the service was added; otherwise, the
        /// original <see cref="IServiceCollection"/>.
        /// </returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedSingletonIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Type implementationType)
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", serviceType.FullName, implementationType.FullName);
            return serviceDescriptors.AddKeyedSingleton(serviceType, serviceKey, implementationType);
        }

        /// <summary>
        /// Adds a Singleton service of the specified type with an implementation factory to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedSingletonIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Func<IServiceProvider, object?, object> implementationFactory)
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", serviceType.FullName, serviceType.FullName);
            return serviceDescriptors.AddKeyedSingleton(serviceType, serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Singleton service of the specified type with an implementation of the specified
        /// type to the service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedSingletonIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey)
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", serviceType.FullName, serviceType.FullName);
            return serviceDescriptors.AddKeyedSingleton(serviceType, serviceKey);
        }

        /// <summary>
        /// Adds a Singleton service of the specified type with a factory specified in <paramref
        /// name="implementationFactory"/> to the service collection if the <paramref
        /// name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedSingletonIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TService> implementationFactory)
            where TService : class
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", typeof(TService).FullName, typeof(TService).FullName);
            return serviceDescriptors.AddKeyedSingleton(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Singleton service of the specified type to the service collection if the
        /// <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedSingletonIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", typeof(TService).FullName, typeof(TService).FullName);
            return serviceDescriptors.AddKeyedSingleton<TService>(serviceKey);
        }

        /// <summary>
        /// Adds a Singleton service of the specified type with an implementation factory to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedSingletonIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", typeof(TImplementation).FullName, typeof(TService).FullName);
            return serviceDescriptors.AddKeyedSingleton<TService, TImplementation>(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Singleton service of the specified type with an implementation type to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedSingletonIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", typeof(TImplementation).FullName, typeof(TService).FullName);
            return serviceDescriptors.AddKeyedSingleton<TService, TImplementation>(serviceKey);
        }

        /// <summary>
        /// Adds a Singleton service of the specified type with an implementation type to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementationType">The implementation type to use.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddSingletonIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Type implementationType)
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton service: {0} as {1}", implementationType.FullName, serviceType.FullName);
            return serviceDescriptors.AddSingleton(serviceType, implementationType);
        }

        /// <summary>
        /// Adds a Singleton service of the specified type with an implementation factory to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddSingletonIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton service: {0}", serviceType.FullName);
            return serviceDescriptors.AddSingleton(serviceType, implementationFactory);
        }

        /// <summary>
        /// Adds a Singleton service of the specified type to the service collection if the
        /// <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddSingletonIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType)
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton service: {0}", serviceType.FullName);
            return serviceDescriptors.AddSingleton(serviceType);
        }

        /// <summary>
        /// Adds a Singleton service of the specified type with a factory specified in <paramref
        /// name="implementationFactory"/> to the service collection if the <paramref
        /// name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddSingletonIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton service: {0}", typeof(TService).FullName);
            return serviceDescriptors.AddSingleton(implementationFactory);
        }

        /// <summary>
        /// Adds a Singleton service of the specified type to the service collection if the
        /// <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddSingletonIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton service: {0}", typeof(TService).FullName);
            return serviceDescriptors.AddSingleton<TService>();
        }

        /// <summary>
        /// Adds a Singleton service of the specified type with an implementation factory to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddSingletonIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton service: {0} as {1}", typeof(TImplementation).FullName, typeof(TService).FullName);
            return serviceDescriptors.AddSingleton<TService, TImplementation>(implementationFactory);
        }

        /// <summary>
        /// Adds a Singleton service of the specified type with an implementation type to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddSingletonIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding singleton service: {0} as {1}", typeof(TImplementation).FullName, typeof(TService).FullName);
            return serviceDescriptors.AddSingleton<TService, TImplementation>();
        }

        /// <summary>
        /// Attempts to register all implementations of the specified service type as Singleton
        /// services in the service collection.
        /// </summary>
        /// <remarks>
        /// This method registers all implementations of the specified service type <typeparamref
        /// name="T"/> as Singleton services. If <paramref name="serviceDescriptors"/> is <see
        /// langword="null"/>, no services are added, and <see langword="null"/> is returned.
        /// </remarks>
        /// <typeparam name="T">The service type to register implementations for.</typeparam>
        /// <param name="serviceDescriptors">
        /// The service collection to which the Singleton services will be added.
        /// </param>
        /// <returns>
        /// The updated <see cref="IServiceCollection"/> if the operation succeeds; otherwise, <see
        /// langword="null"/> if <paramref name="serviceDescriptors"/> is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? TryAddAllSingleton<T>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.TryAddAllSingleton(typeof(T));

        /// <summary>
        /// Attempts to add all available types that are assignable to the specified type as
        /// Singleton services to the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// This method scans the available types in the application's configuration and registers
        /// all types that are assignable to <paramref name="registerType"/> as Singleton services.
        /// If <paramref name="registerType"/> is a generic type definition, it will also be
        /// registered as itself.
        /// </remarks>
        /// <param name="serviceDescriptors">
        /// The <see cref="IServiceCollection"/> to which the services will be added. Cannot be null.
        /// </param>
        /// <param name="registerType">
        /// The type to register as a Singleton service. This can be a concrete type or an
        /// interface. If it is a generic type definition, it will be registered as itself.
        /// </param>
        /// <returns>
        /// The updated <see cref="IServiceCollection"/> with the added Singleton services, or <see
        /// langword="null"/> if <paramref name="serviceDescriptors"/> is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? TryAddAllSingleton(this IServiceCollection? serviceDescriptors, Type registerType)
        {
            if (serviceDescriptors is null)
                return serviceDescriptors;
            var CanisterConfiguration = GetCanisterConfiguration(serviceDescriptors);
            foreach (Type? TempType in CanisterConfiguration.AvailableTypes.Where(registerType.IsAssignableFrom))
            {
                CanisterConfiguration.Log("Adding singleton service: {0} as {1}", TempType.FullName, TempType.FullName);
                serviceDescriptors.TryAddSingleton(TempType, TempType);
                CanisterConfiguration.Log("Adding singleton service: {0} as {1}", TempType.FullName, registerType.FullName);
                serviceDescriptors.TryAddSingleton(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
            {
                CanisterConfiguration.Log("Adding singleton service: {0} as {1}", registerType.FullName, registerType.FullName);
                serviceDescriptors.TryAddSingleton(registerType, registerType);
            }
            return serviceDescriptors;
        }
    }
}