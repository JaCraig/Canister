using Canister.BaseClasses;
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
            Register<DefaultBootstrapper>(this);
            Register<IBootstrapper>(this);
        }

        /// <summary>
        /// The IoC container
        /// </summary>
        public override IServiceCollection AppContainer { get; }

        /// <summary>
        /// Name of the bootstrapper
        /// </summary>
        public override string Name => "Default bootstrapper";

        /// <summary>
        /// The <see cref="T:System.IServiceProvider"/> used to resolve dependencies from the scope.
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Gets the available types.
        /// </summary>
        /// <value>The available types.</value>
        private Type[] AvailableTypes { get; set; }

        /// <summary>
        /// Creates the service scope.
        /// </summary>
        /// <returns>The service scope</returns>
        public IServiceScope CreateScope()
        {
            return ServiceProvider.CreateScope();
        }

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
            if (lifeTime == ServiceLifetime.Scoped)
                AppContainer.AddScoped(_ => objectToRegister);
            else if (lifeTime == ServiceLifetime.Singleton)
                AppContainer.AddSingleton(_ => objectToRegister);
            else
                AppContainer.AddTransient(_ => objectToRegister);
            if (AvailableTypes == null)
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
            if (lifeTime == ServiceLifetime.Scoped)
                AppContainer.AddScoped<T>();
            else if (lifeTime == ServiceLifetime.Singleton)
                AppContainer.AddSingleton<T>();
            else
                AppContainer.AddTransient<T>();
            if (AvailableTypes == null)
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
            if (lifeTime == ServiceLifetime.Scoped)
                AppContainer.AddScoped<T1, T2>();
            else if (lifeTime == ServiceLifetime.Singleton)
                AppContainer.AddSingleton<T1, T2>();
            else
                AppContainer.AddTransient<T1, T2>();
            if (AvailableTypes == null)
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
            if (lifeTime == ServiceLifetime.Scoped)
                AppContainer.AddScoped(function);
            else if (lifeTime == ServiceLifetime.Singleton)
                AppContainer.AddSingleton(function);
            else
                AppContainer.AddTransient(function);
            if (AvailableTypes == null)
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
            if (lifeTime == ServiceLifetime.Scoped)
                AppContainer.AddScoped(objectType);
            else if (lifeTime == ServiceLifetime.Singleton)
                AppContainer.AddSingleton(objectType);
            else
                AppContainer.AddTransient(objectType);
            if (AvailableTypes == null)
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
            foreach (Type TempType in GetAvailableTypes().Where(x => IsOfType(x, typeof(T))))
            {
                if (lifeTime == ServiceLifetime.Scoped)
                {
                    AppContainer.AddScoped(TempType, TempType);
                    AppContainer.AddScoped(RegisterType, TempType);
                }
                else if (lifeTime == ServiceLifetime.Singleton)
                {
                    AppContainer.AddSingleton(TempType, TempType);
                    AppContainer.AddSingleton(RegisterType, TempType);
                }
                else
                {
                    AppContainer.AddTransient(TempType, TempType);
                    AppContainer.AddTransient(RegisterType, TempType);
                }
            }
            if (AvailableTypes == null)
                UpdateServiceProvider();
            return this;
        }

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        public override T Resolve<T>(T defaultObject = null)
        {
            return ServiceProvider.GetRequiredService<T>() ?? defaultObject;
        }

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        public override T Resolve<T>(string name, T defaultObject = null)
        {
            return ServiceProvider.GetRequiredService<T>() ?? defaultObject;
        }

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        public override object Resolve(Type objectType, object defaultObject = null)
        {
            return ServiceProvider.GetRequiredService(objectType) ?? defaultObject;
        }

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        public override object Resolve(Type objectType, string name, object defaultObject = null)
        {
            return ServiceProvider.GetRequiredService(objectType) ?? defaultObject;
        }

        /// <summary>
        /// Resolves the objects based on the type specified
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>A list of objects of the specified type</returns>
        public override IEnumerable<T> ResolveAll<T>()
        {
            return ServiceProvider.GetRequiredService<IEnumerable<T>>() ?? Array.Empty<T>();
        }

        /// <summary>
        /// Resolves all objects based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A list of objects of the specified type</returns>
        public override IEnumerable<object> ResolveAll(Type objectType)
        {
            return ServiceProvider.GetRequiredService(typeof(IEnumerable<>).MakeGenericType(objectType)) as IEnumerable<object> ?? Array.Empty<object>();
        }

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
        protected override void BeforeBuild()
        {
            AvailableTypes = GetAvailableTypes();

            ServiceProvider = new DefaultServiceProviderFactory().CreateServiceProvider(AppContainer);
        }

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
                         catch { return new Type[0]; }
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
            if (ServiceProvider != null)
                ServiceProvider = new DefaultServiceProviderFactory().CreateServiceProvider(AppContainer);
        }
    }
}