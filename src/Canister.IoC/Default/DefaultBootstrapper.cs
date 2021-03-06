﻿using Canister.BaseClasses;
using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Canister.Default
{
    /// <summary>
    /// Default bootstrapper
    /// </summary>
    /// <seealso cref="BootstrapperBase{IServiceCollection}"/>
    /// <seealso cref="IScope"/>
    public class DefaultBootstrapper : BootstrapperBase<IServiceCollection>, IScope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBootstrapper"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="collection">The collection.</param>
        public DefaultBootstrapper(IEnumerable<Assembly> assemblies, IEnumerable<ServiceDescriptor> collection)
            : base(assemblies)
        {
            AppContainer = (IServiceCollection)collection;
            Register(this);
            Register<IBootstrapper>(this);
            ServiceProviderFactory = new DefaultServiceProviderFactory();
        }

        /// <summary>
        /// The IoC container
        /// </summary>
        public override IServiceCollection AppContainer { get; }

        /// <summary>
        /// Name of the bootstrapper
        /// </summary>
        public override string Name { get; } = "Default bootstrapper";

        /// <summary>
        /// The <see cref="T:System.IServiceProvider"/> used to resolve dependencies from the scope.
        /// </summary>
        public IServiceProvider? ServiceProvider { get; private set; }

        /// <summary>
        /// The service provider factory
        /// </summary>
        private DefaultServiceProviderFactory ServiceProviderFactory { get; }

        /// <summary>
        /// Gets the available types.
        /// </summary>
        /// <value>The available types.</value>
        private Type[]? AvailableTypes;

        /// <summary>
        /// Creates the service scope.
        /// </summary>
        /// <returns>The service scope</returns>
        public IServiceScope? CreateScope() => ServiceProvider?.CreateScope();

        /// <summary>
        /// Registers an object with the bootstrapper
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="objectToRegister">The object to register.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public override IBootstrapper Register<T>(T objectToRegister, ServiceLifetime lifeTime = ServiceLifetime.Singleton, string name = "")
        {
            switch (lifeTime)
            {
                case ServiceLifetime.Scoped:
                    AppContainer.AddScoped(_ => objectToRegister);
                    break;

                case ServiceLifetime.Singleton:
                    AppContainer.AddSingleton(_ => objectToRegister);
                    break;

                default:
                    AppContainer.AddTransient(_ => objectToRegister);
                    break;
            }
            if (AvailableTypes is null)
                UpdateServiceProvider();
            return this;
        }

        /// <summary>
        /// Registers a type with the default constructor
        /// </summary>
        /// <typeparam name="T">Object type to register</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public override IBootstrapper Register<T>(ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
        {
            switch (lifeTime)
            {
                case ServiceLifetime.Scoped:
                    AppContainer.AddScoped<T>();
                    break;

                case ServiceLifetime.Singleton:
                    AppContainer.AddSingleton<T>();
                    break;

                default:
                    AppContainer.AddTransient<T>();
                    break;
            }
            if (AvailableTypes is null)
                UpdateServiceProvider();
            return this;
        }

        /// <summary>
        /// Registers a type with the default constructor of a child class
        /// </summary>
        /// <typeparam name="T1">Base class/interface type</typeparam>
        /// <typeparam name="T2">Child class type</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public override IBootstrapper Register<T1, T2>(ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
        {
            switch (lifeTime)
            {
                case ServiceLifetime.Scoped:
                    AppContainer.AddScoped<T1, T2>();
                    break;

                case ServiceLifetime.Singleton:
                    AppContainer.AddSingleton<T1, T2>();
                    break;

                default:
                    AppContainer.AddTransient<T1, T2>();
                    break;
            }
            if (AvailableTypes is null)
                UpdateServiceProvider();
            return this;
        }

        /// <summary>
        /// Registers a type with a function
        /// </summary>
        /// <typeparam name="T">Type that the function returns</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public override IBootstrapper Register<T>(Func<IServiceProvider, object> function, ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
        {
            switch (lifeTime)
            {
                case ServiceLifetime.Scoped:
                    AppContainer.AddScoped(function);
                    break;

                case ServiceLifetime.Singleton:
                    AppContainer.AddSingleton(function);
                    break;

                default:
                    AppContainer.AddTransient(function);
                    break;
            }
            if (AvailableTypes is null)
                UpdateServiceProvider();
            return this;
        }

        /// <summary>
        /// Registers a generic type with the default constructor
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public override IBootstrapper Register(Type objectType, ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
        {
            switch (lifeTime)
            {
                case ServiceLifetime.Scoped:
                    AppContainer.AddScoped(objectType);
                    break;

                case ServiceLifetime.Singleton:
                    AppContainer.AddSingleton(objectType);
                    break;

                default:
                    AppContainer.AddTransient(objectType);
                    break;
            }
            if (AvailableTypes is null)
                UpdateServiceProvider();
            return this;
        }

        /// <summary>
        /// Registers all objects of a certain type with the bootstrapper
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <returns>This</returns>
        public override IBootstrapper RegisterAll<T>(ServiceLifetime lifeTime = ServiceLifetime.Transient)
        {
            var RegisterType = typeof(T);
            foreach (var TempType in GetAvailableTypes().Where(x => IsOfType(x, typeof(T))))
            {
                switch (lifeTime)
                {
                    case ServiceLifetime.Scoped:
                        AppContainer.AddScoped(TempType, TempType);
                        AppContainer.AddScoped(RegisterType, TempType);
                        break;

                    case ServiceLifetime.Singleton:
                        AppContainer.AddSingleton(TempType, TempType);
                        AppContainer.AddSingleton(RegisterType, TempType);
                        break;

                    default:
                        AppContainer.AddTransient(TempType, TempType);
                        AppContainer.AddTransient(RegisterType, TempType);
                        break;
                }
            }
            if (AvailableTypes is null)
                UpdateServiceProvider();
            return this;
        }

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        public override T Resolve<T>(T? defaultObject = null)
            where T : class => ServiceProvider.GetRequiredService<T>();

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        public override T Resolve<T>(string name, T? defaultObject = null)
            where T : class => ServiceProvider.GetRequiredService<T>();

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        public override object Resolve(Type objectType, object? defaultObject = null) => ServiceProvider.GetRequiredService(objectType);

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        public override object Resolve(Type objectType, string name, object? defaultObject = null) => ServiceProvider.GetRequiredService(objectType);

        /// <summary>
        /// Resolves the objects based on the type specified
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>A list of objects of the specified type</returns>
        public override IEnumerable<T> ResolveAll<T>() => ServiceProvider.GetRequiredService<IEnumerable<T>>();

        /// <summary>
        /// Resolves all objects based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A list of objects of the specified type</returns>
        public override IEnumerable<object> ResolveAll(Type objectType) => ServiceProvider.GetRequiredService(typeof(IEnumerable<>).MakeGenericType(objectType)) as IEnumerable<object> ?? Array.Empty<object>();

        /// <summary>
        /// Called after the build step.
        /// </summary>
        protected override void AfterBuild()
        {
            AvailableTypes = null;
            ServiceProvider = new DefaultServiceProviderFactory().CreateServiceProvider(AppContainer);
        }

        /// <summary>
        /// Called before the build step.
        /// </summary>
        protected override void BeforeBuild() => AvailableTypes = GetAvailableTypes();

        /// <summary>
        /// Runs before the module resolve.
        /// </summary>
        protected override void BeforeModuleResolve() => ServiceProvider = new DefaultServiceProviderFactory().CreateServiceProvider(AppContainer);

        /// <summary>
        /// Gets the available types.
        /// </summary>
        /// <returns>The available types.</returns>
        private Type[] GetAvailableTypes()
        {
            return AvailableTypes
                ?? Assemblies.SelectMany(x =>
                     {
                         try
                         {
                             return x.GetTypes();
                         }
                         catch (ReflectionTypeLoadException) { return Array.Empty<Type>(); }
                     }).Where(x => x.IsClass
                         && !x.IsAbstract
                         && !x.ContainsGenericParameters)
                    .ToArray();
        }

        /// <summary>
        /// Updates the service provider.
        /// </summary>
        private void UpdateServiceProvider()
        {
            if (ServiceProvider is null)
                return;
            ServiceProvider = ServiceProviderFactory.CreateServiceProvider(AppContainer);
        }
    }
}