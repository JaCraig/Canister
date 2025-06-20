using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for decorating registered services.
    /// </summary>
    public static class ServiceCollectionDecoratorExtensions
    {
        /// <summary>
        /// Decorates all registrations of TService with TDecorator. The decorator must have a
        /// constructor that takes TService as a parameter.
        /// </summary>
        /// <typeparam name="TService">The service type to decorate.</typeparam>
        /// <typeparam name="TDecorator">The decorator type (must implement TService).</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        [return: NotNullIfNotNull(nameof(services))]
        public static IServiceCollection? Decorate<TService, TDecorator>(this IServiceCollection? services)
            where TService : class
            where TDecorator : class, TService
        {
            if (services is null)
                return null;
            var Descriptors = services.Where(sd => sd.ServiceType == typeof(TService)).ToList();
            if (Descriptors.Count == 0)
                return services;

            foreach (var Descriptor in Descriptors)
            {
                ServiceDescriptor? DecoratedDescriptor = Descriptor switch
                {
                    { ImplementationType: not null } D => ServiceDescriptor.Describe(
                        typeof(TService),
                        provider => (TService)Activator.CreateInstance(typeof(TDecorator),
                            ActivatorUtilities.GetServiceOrCreateInstance(provider, D.ImplementationType))!,
                        D.Lifetime),
                    { ImplementationFactory: not null } D => ServiceDescriptor.Describe(
                        typeof(TService),
                        provider => (TService)Activator.CreateInstance(typeof(TDecorator),
                            D.ImplementationFactory(provider))!,
                        D.Lifetime),
                    { ImplementationInstance: not null } D => ServiceDescriptor.Describe(
                        typeof(TService),
                        _ => (TService)Activator.CreateInstance(typeof(TDecorator), D.ImplementationInstance)!,
                        D.Lifetime),
                    _ => null
                };
                if (DecoratedDescriptor is null)
                    continue;
                services.Remove(Descriptor);
                services.Add(DecoratedDescriptor);
            }
            return services;
        }
    }
}