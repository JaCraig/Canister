using Fast.Activator;
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
                services.Remove(Descriptor);

                if (Descriptor.IsKeyedService)
                {
                    var ServiceKey = Descriptor.ServiceKey;
                    switch (Descriptor.Lifetime)
                    {
                        case ServiceLifetime.Scoped:
                            services.AddKeyedScoped(typeof(TService), ServiceKey,
                                (provider, key) => (TService)FastActivator.CreateInstance(
                                    typeof(TDecorator),
                                    ResolveInner(provider, Descriptor, key))!);
                            break;

                        case ServiceLifetime.Singleton:
                            services.AddKeyedSingleton(typeof(TService), ServiceKey,
                                (provider, key) => (TService)FastActivator.CreateInstance(
                                    typeof(TDecorator),
                                    ResolveInner(provider, Descriptor, key))!);
                            break;

                        case ServiceLifetime.Transient:
                            services.AddKeyedTransient(typeof(TService), ServiceKey,
                                (provider, key) => (TService)FastActivator.CreateInstance(
                                    typeof(TDecorator),
                                    ResolveInner(provider, Descriptor, key))!);
                            break;
                    }

                    continue;
                }

                switch (Descriptor.Lifetime)
                {
                    case ServiceLifetime.Scoped:
                        services.AddScoped(typeof(TService),
                            provider => (TService)FastActivator.CreateInstance(
                                typeof(TDecorator),
                                ResolveInner(provider, Descriptor, null))!);
                        break;

                    case ServiceLifetime.Singleton:
                        services.AddSingleton(typeof(TService),
                            provider => (TService)FastActivator.CreateInstance(
                                typeof(TDecorator),
                                ResolveInner(provider, Descriptor, null))!);
                        break;

                    case ServiceLifetime.Transient:
                        services.AddTransient(typeof(TService),
                            provider => (TService)FastActivator.CreateInstance(
                                typeof(TDecorator),
                                ResolveInner(provider, Descriptor, null))!);
                        break;
                }
            }

            return services;
        }

        private static object ResolveInner(IServiceProvider provider, ServiceDescriptor descriptor, object? key)
        {
            if (descriptor.IsKeyedService)
            {
                if (descriptor.KeyedImplementationType is not null)
                    return ActivatorUtilities.GetServiceOrCreateInstance(provider, descriptor.KeyedImplementationType);
                if (descriptor.KeyedImplementationFactory is not null)
                    return descriptor.KeyedImplementationFactory(provider, key);
                if (descriptor.KeyedImplementationInstance is not null)
                    return descriptor.KeyedImplementationInstance;
            }
            else
            {
                if (descriptor.ImplementationType is not null)
                    return ActivatorUtilities.GetServiceOrCreateInstance(provider, descriptor.ImplementationType);
                if (descriptor.ImplementationFactory is not null)
                    return descriptor.ImplementationFactory(provider);
                if (descriptor.ImplementationInstance is not null)
                    return descriptor.ImplementationInstance;
            }

            throw new InvalidOperationException("Unsupported service descriptor registration for decoration.");
        }
    }
}