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
    /// Transient lifetimes, including conditional and keyed registrations.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all objects of a certain type with the service collection as Transient.
        /// </summary>
        /// <typeparam name="T">The object type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the services to.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddAllTransient<T>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.AddAllTransient(typeof(T));

        /// <summary>
        /// Registers all objects of a certain type with the service collection as Transient.
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
                CanisterConfiguration.Log("Adding transient service: {0} as {1}", TempType.FullName, TempType.FullName);
                _ = serviceDescriptors.AddTransient(TempType, TempType);
                CanisterConfiguration.Log("Adding transient service: {0} as {1}", TempType.FullName, registerType.FullName);
                _ = serviceDescriptors.AddTransient(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
            {
                CanisterConfiguration.Log("Adding transient service: {0} as {1}", registerType.FullName, registerType.FullName);
                _ = serviceDescriptors.AddTransient(registerType, registerType);
            }
            return serviceDescriptors;
        }

        /// <summary>
        /// Adds a Transient service with a specified key to the service collection if the given
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
        public static IServiceCollection? AddKeyedTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Type implementationType)
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", serviceType.FullName, implementationType.FullName);
            return serviceDescriptors.AddKeyedTransient(serviceType, serviceKey, implementationType);
        }

        /// <summary>
        /// Adds a Transient service of the specified type with an implementation factory to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Func<IServiceProvider, object?, object> implementationFactory)
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", serviceType.FullName, serviceType.FullName);
            return serviceDescriptors.AddKeyedTransient(serviceType, serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Transient service of the specified type with an implementation of the specified
        /// type to the service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey)
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", serviceType.FullName, serviceType.FullName);
            return serviceDescriptors.AddKeyedTransient(serviceType, serviceKey);
        }

        /// <summary>
        /// Adds a Transient service of the specified type with a factory specified in <paramref
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
        public static IServiceCollection? AddKeyedTransientIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TService> implementationFactory)
            where TService : class
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", typeof(TService).FullName, typeof(TService).FullName);
            return serviceDescriptors.AddKeyedTransient(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Transient service of the specified type to the service collection if the
        /// <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedTransientIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", typeof(TService).FullName, typeof(TService).FullName);
            return serviceDescriptors.AddKeyedTransient<TService>(serviceKey);
        }

        /// <summary>
        /// Adds a Transient service of the specified type with an implementation factory to the
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
        public static IServiceCollection? AddKeyedTransientIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", typeof(TImplementation).FullName, typeof(TService).FullName);
            return serviceDescriptors.AddKeyedTransient<TService, TImplementation>(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a Transient service of the specified type with an implementation type to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddKeyedTransientIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient keyed service: {key}: {0} as {1}", serviceKey?.ToString() ?? "null", typeof(TImplementation).FullName, typeof(TService).FullName);
            return serviceDescriptors.AddKeyedTransient<TService, TImplementation>(serviceKey);
        }

        /// <summary>
        /// Adds a Transient service of the specified type with an implementation type to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementationType">The implementation type to use.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Type implementationType)
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient service: {0} as {1}", implementationType.FullName, serviceType.FullName);
            return serviceDescriptors.AddTransient(serviceType, implementationType);
        }

        /// <summary>
        /// Adds a Transient service of the specified type with an implementation factory to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient service: {0}", serviceType.FullName);
            return serviceDescriptors.AddTransient(serviceType, implementationFactory);
        }

        /// <summary>
        /// Adds a Transient service of the specified type to the service collection if the
        /// <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddTransientIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType)
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient service: {0}", serviceType.FullName);
            return serviceDescriptors.AddTransient(serviceType);
        }

        /// <summary>
        /// Adds a Transient service of the specified type with a factory specified in <paramref
        /// name="implementationFactory"/> to the service collection if the <paramref
        /// name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddTransientIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient service: {0}", typeof(TService).FullName);
            return serviceDescriptors.AddTransient(implementationFactory);
        }

        /// <summary>
        /// Adds a Transient service of the specified type to the service collection if the
        /// <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddTransientIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient service: {0}", typeof(TService).FullName);
            return serviceDescriptors.AddTransient<TService>();
        }

        /// <summary>
        /// Adds a Transient service of the specified type with an implementation factory to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? AddTransientIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient service: {0} as {1}", typeof(TImplementation).FullName, typeof(TService).FullName);
            return serviceDescriptors.AddTransient<TService, TImplementation>(implementationFactory);
        }

        /// <summary>
        /// Adds a Transient service of the specified type with an implementation type to the
        /// service collection if the <paramref name="predicate"/> is true.
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
            if (serviceDescriptors is null || !predicate(serviceDescriptors))
                return serviceDescriptors;
            GetCanisterConfiguration(serviceDescriptors)?.Log("Adding transient service: {0} as {1}", typeof(TImplementation).FullName, typeof(TService).FullName);
            return serviceDescriptors.AddTransient<TService, TImplementation>();
        }

        /// <summary>
        /// Attempts to register all implementations of the specified service type as Transient
        /// services in the service collection.
        /// </summary>
        /// <remarks>
        /// This method registers all implementations of the specified service type <typeparamref
        /// name="T"/> as Transient services. If <paramref name="serviceDescriptors"/> is <see
        /// langword="null"/>, no services are added, and <see langword="null"/> is returned.
        /// </remarks>
        /// <typeparam name="T">The service type to register implementations for.</typeparam>
        /// <param name="serviceDescriptors">
        /// The service collection to which the Transient services will be added.
        /// </param>
        /// <returns>
        /// The updated <see cref="IServiceCollection"/> if the operation succeeds; otherwise, <see
        /// langword="null"/> if <paramref name="serviceDescriptors"/> is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? TryAddAllTransient<T>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.TryAddAllTransient(typeof(T));

        /// <summary>
        /// Attempts to add all available types that are assignable to the specified type as
        /// Transient services to the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// This method scans the available types in the application's configuration and registers
        /// all types that are assignable to <paramref name="registerType"/> as Transient services.
        /// If <paramref name="registerType"/> is a generic type definition, it will also be
        /// registered as itself.
        /// </remarks>
        /// <param name="serviceDescriptors">
        /// The <see cref="IServiceCollection"/> to which the services will be added. Cannot be null.
        /// </param>
        /// <param name="registerType">
        /// The type to register as a Transient service. This can be a concrete type or an
        /// interface. If it is a generic type definition, it will be registered as itself.
        /// </param>
        /// <returns>
        /// The updated <see cref="IServiceCollection"/> with the added Transient services, or <see
        /// langword="null"/> if <paramref name="serviceDescriptors"/> is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull(nameof(serviceDescriptors))]
        public static IServiceCollection? TryAddAllTransient(this IServiceCollection? serviceDescriptors, Type registerType)
        {
            if (serviceDescriptors is null)
                return serviceDescriptors;
            var CanisterConfiguration = GetCanisterConfiguration(serviceDescriptors);
            foreach (Type? TempType in CanisterConfiguration.AvailableTypes.Where(registerType.IsAssignableFrom))
            {
                CanisterConfiguration.Log("Adding transient service: {0} as {1}", TempType.FullName, TempType.FullName);
                serviceDescriptors.TryAddTransient(TempType, TempType);
                CanisterConfiguration.Log("Adding transient service: {0} as {1}", TempType.FullName, registerType.FullName);
                serviceDescriptors.TryAddTransient(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
            {
                CanisterConfiguration.Log("Adding transient service: {0} as {1}", registerType.FullName, registerType.FullName);
                serviceDescriptors.TryAddTransient(registerType, registerType);
            }
            return serviceDescriptors;
        }
    }
}