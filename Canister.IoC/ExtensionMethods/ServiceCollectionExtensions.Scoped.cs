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

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/> to register services with
    /// scoped lifetimes, including conditional and keyed registrations.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all objects of a certain type with the service collection as scoped.
        /// </summary>
        /// <typeparam name="T">The object type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the services to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddAllScoped<T>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.AddAllScoped(typeof(T));

        /// <summary>
        /// Registers all objects of a certain type with the service collection as scoped.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the services to.</param>
        /// <param name="registerType">The type to register.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddAllScoped(this IServiceCollection? serviceDescriptors, Type registerType)
        {
            if (serviceDescriptors is null)
                return serviceDescriptors;
            foreach (Type? TempType in GetCanisterConfiguration(serviceDescriptors).AvailableTypes.Where(registerType.IsAssignableFrom))
            {
                _ = serviceDescriptors.AddScoped(TempType, TempType);
                _ = serviceDescriptors.AddScoped(registerType, TempType);
            }
            if (registerType.IsGenericTypeDefinition)
                _ = serviceDescriptors.AddScoped(registerType, registerType);
            return serviceDescriptors;
        }

        /// <summary>
        /// Adds a scoped service with a specified key to the service collection if the given
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
        public static IServiceCollection? AddKeyedScopedIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Type implementationType)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedScoped(serviceType, serviceKey, implementationType);
        }

        /// <summary>
        /// Adds a scoped service of the specified type with an implementation factory to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddKeyedScopedIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey, Func<IServiceProvider, object?, object> implementationFactory)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedScoped(serviceType, serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a scoped service of the specified type with an implementation of the specified type
        /// to the service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddKeyedScopedIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, object? serviceKey) => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddKeyedScoped(serviceType, serviceKey);

        /// <summary>
        /// Adds a scoped service of the specified type with a factory specified in <paramref
        /// name="implementationFactory"/> to the service collection if the <paramref
        /// name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddKeyedScopedIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TService> implementationFactory)
            where TService : class
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedScoped(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a scoped service of the specified type to the service collection if the <paramref
        /// name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddKeyedScopedIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddKeyedScoped<TService>(serviceKey);

        /// <summary>
        /// Adds a scoped service of the specified type with an implementation factory to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddKeyedScopedIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey, Func<IServiceProvider, object?, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedScoped<TService, TImplementation>(serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a scoped service of the specified type with an implementation type to the service
        /// collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceKey">The key for the service registration.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddKeyedScopedIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, object? serviceKey)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddKeyedScoped<TService, TImplementation>(serviceKey);
        }

        /// <summary>
        /// Adds a scoped service of the specified type with an implementation type to the service
        /// collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementationType">The implementation type to use.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddScopedIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Type implementationType)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddScoped(serviceType, implementationType);
        }

        /// <summary>
        /// Adds a scoped service of the specified type with an implementation factory to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddScopedIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddScoped(serviceType, implementationFactory);
        }

        /// <summary>
        /// Adds a scoped service of the specified type to the service collection if the <paramref
        /// name="predicate"/> is true.
        /// </summary>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="serviceType">The service type to register.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddScopedIf(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Type serviceType) => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddScoped(serviceType);

        /// <summary>
        /// Adds a scoped service of the specified type with a factory specified in <paramref
        /// name="implementationFactory"/> to the service collection if the <paramref
        /// name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddScopedIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddScoped(implementationFactory);
        }

        /// <summary>
        /// Adds a scoped service of the specified type to the service collection if the <paramref
        /// name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddScopedIf<TService>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class => serviceDescriptors is null || !predicate(serviceDescriptors) ? serviceDescriptors : serviceDescriptors.AddScoped<TService>();

        /// <summary>
        /// Adds a scoped service of the specified type with an implementation factory to the
        /// service collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <param name="implementationFactory">The factory to create the implementation.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddScopedIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddScoped<TService, TImplementation>(implementationFactory);
        }

        /// <summary>
        /// Adds a scoped service of the specified type with an implementation type to the service
        /// collection if the <paramref name="predicate"/> is true.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to use.</typeparam>
        /// <param name="serviceDescriptors">The service collection to add the service to.</param>
        /// <param name="predicate">A predicate to determine if the service should be added.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? AddScopedIf<TService, TImplementation>(this IServiceCollection? serviceDescriptors, Func<IServiceCollection, bool> predicate)
            where TService : class
            where TImplementation : class, TService
        {
            return serviceDescriptors is null || !predicate(serviceDescriptors)
                ? serviceDescriptors
                : serviceDescriptors.AddScoped<TService, TImplementation>();
        }
    }
}