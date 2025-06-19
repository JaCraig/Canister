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
    }
}