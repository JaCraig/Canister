using Canister.Interfaces;
using Canister.IoC.Attributes;
using Fast.Activator;
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
    /// Extension methods for <see cref="IServiceCollection"/> to check for the existence of services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Determines if the specified service type exists in the service collection.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <returns><c>true</c> if the service type exists; otherwise, <c>false</c>.</returns>
        public static bool Exists<TService>(this IServiceCollection? serviceDescriptors) => serviceDescriptors?.Exists(typeof(TService)) ?? false;

        /// <summary>
        /// Determines if the specified service type exists in the service collection.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="serviceType">The type of the service.</param>
        /// <returns><c>true</c> if the service type exists; otherwise, <c>false</c>.</returns>
        public static bool Exists(this IServiceCollection? serviceDescriptors, Type serviceType) => serviceDescriptors?.Any(service => service.ServiceType == serviceType) ?? false;

        /// <summary>
        /// Determines if the specified keyed service type exists in the service collection.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="serviceKey">The service key.</param>
        /// <returns><c>true</c> if the keyed service type exists; otherwise, <c>false</c>.</returns>
        public static bool Exists(this IServiceCollection? serviceDescriptors, Type serviceType, object? serviceKey) => serviceDescriptors?.Any(service => service.IsKeyedService && service.ServiceType == serviceType && service.ServiceKey == serviceKey) ?? false;

        /// <summary>
        /// Determines if the specified keyed service type exists in the service collection.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="serviceKey">The service key.</param>
        /// <returns><c>true</c> if the keyed service type exists; otherwise, <c>false</c>.</returns>
        public static bool Exists<TService>(this IServiceCollection? serviceDescriptors, object? serviceKey) => serviceDescriptors?.Exists(typeof(TService), serviceKey) ?? false;

        /// <summary>
        /// Determines if the specified service and implementation types exist in the service collection.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <returns><c>true</c> if the service and implementation types exist; otherwise, <c>false</c>.</returns>
        public static bool Exists<TService, TImplementation>(this IServiceCollection? serviceDescriptors)
            where TService : class
            where TImplementation : class, TService => serviceDescriptors?.Exists(typeof(TService), typeof(TImplementation)) ?? false;

        /// <summary>
        /// Determines if the specified service and implementation types exist in the service collection.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="implementationType">The type of the implementation.</param>
        /// <returns><c>true</c> if the service and implementation types exist; otherwise, <c>false</c>.</returns>
        public static bool Exists(this IServiceCollection? serviceDescriptors, Type serviceType, Type implementationType) => serviceDescriptors?.Any(service => service.ServiceType == serviceType && service.ImplementationType == implementationType) ?? false;
    }
}