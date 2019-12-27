using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Canister.IoC.Default
{
    /// <summary>
    /// Cansiter service provider factory
    /// </summary>
    /// <seealso cref="IServiceProviderFactory{IBootstrapper}"/>
    public class CanisterServiceProviderFactory : IServiceProviderFactory<IBootstrapper>
    {
        /// <summary>
        /// Creates a container builder from an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <returns>A container builder that can be used to create an <see cref="T:System.IServiceProvider"/>.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IBootstrapper CreateBuilder(IServiceCollection services)
        {
            return Builder.CreateContainer(services);
        }

        /// <summary>
        /// Creates an <see cref="T:System.IServiceProvider"/> from the container builder.
        /// </summary>
        /// <param name="containerBuilder">The container builder</param>
        /// <returns>An <see cref="T:System.IServiceProvider"/></returns>
        public IServiceProvider CreateServiceProvider(IBootstrapper containerBuilder)
        {
            containerBuilder.Build();
            return containerBuilder;
        }
    }
}