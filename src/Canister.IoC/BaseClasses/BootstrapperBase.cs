/*
Copyright 2016 James Craig

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Canister.BaseClasses
{
    /// <summary>
    /// Bootstrapper base class
    /// </summary>
    /// <typeparam name="Container">The actual IoC object</typeparam>
    public abstract class BootstrapperBase<Container> : IBootstrapper
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        protected BootstrapperBase(IEnumerable<Assembly> assemblies)
        {
            Assemblies = assemblies.ToList();
        }

        /// <summary>
        /// Name of the bootstrapper
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The IoC container
        /// </summary>
        protected abstract Container AppContainer { get; }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <value>The assemblies.</value>
        protected List<Assembly> Assemblies { get; set; }

        /// <summary>
        /// Adds the assembly.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>This</returns>
        public IBootstrapper AddAssembly(params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
                return this;
            foreach (Assembly TempAssembly in assemblies)
            {
                if (!Assemblies.Contains(TempAssembly))
                {
                    Assemblies.Add(TempAssembly);
                }
            }
            return this;
        }

        /// <summary>
        /// Builds this instance, loads all modules, etc.
        /// </summary>
        /// <returns>This</returns>
        public IBootstrapper Build()
        {
            Register<IEnumerable<Assembly>>(Assemblies);
            RegisterAll<IModule>();
            foreach (IModule ResolvedModule in ResolveAll<IModule>().OrderBy(x => x.Order))
            {
                ResolvedModule.Load(this);
            }
            return this;
        }

        /// <summary>
        /// Disposes of the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>The service specified</returns>
        public object GetService(Type serviceType)
        {
            return Resolve(serviceType, "", null);
        }

        /// <summary>
        /// Registers an object with the bootstrapper
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="objectToRegister">The object to register.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public abstract IBootstrapper Register<T>(T objectToRegister, ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
            where T : class;

        /// <summary>
        /// Registers a type with the default constructor
        /// </summary>
        /// <typeparam name="T">Object type to register</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public abstract IBootstrapper Register<T>(ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
            where T : class;

        /// <summary>
        /// Registers a type with the default constructor of a child class
        /// </summary>
        /// <typeparam name="T1">Base class/interface type</typeparam>
        /// <typeparam name="T2">Child class type</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public abstract IBootstrapper Register<T1, T2>(ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
            where T2 : class, T1
            where T1 : class;

        /// <summary>
        /// Registers a type with a function
        /// </summary>
        /// <typeparam name="T">Type that the function returns</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public abstract IBootstrapper Register<T>(Func<IServiceProvider, object> function, ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "") where T : class;

        /// <summary>
        /// Registers a generic type with the default constructor
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public abstract IBootstrapper Register(Type objectType, ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "");

        /// <summary>
        /// Registers all objects of a certain type with the bootstrapper
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <returns>This</returns>
        public abstract IBootstrapper RegisterAll<T>(ServiceLifetime lifeTime = ServiceLifetime.Transient)
            where T : class;

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        public abstract T Resolve<T>(T defaultObject = default(T))
            where T : class;

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        public abstract T Resolve<T>(string name, T defaultObject = default(T))
            where T : class;

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        public abstract object Resolve(Type objectType, object defaultObject = null);

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        public abstract object Resolve(Type objectType, string name, object defaultObject = null);

        /// <summary>
        /// Resolves the objects based on the type specified
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>A list of objects of the specified type</returns>
        public abstract IEnumerable<T> ResolveAll<T>()
            where T : class;

        /// <summary>
        /// Resolves all objects based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A list of objects of the specified type</returns>
        public abstract IEnumerable<object> ResolveAll(Type objectType);

        /// <summary>
        /// Disposes of the object
        /// </summary>
        /// <param name="managed">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        /// unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool managed)
        {
        }

        /// <summary>
        /// Determines whether [is of type] [the specified type].
        /// </summary>
        /// <param name="x">The type to check.</param>
        /// <param name="type">The type to check against.</param>
        /// <returns><c>true</c> if [is of type] [the specified type]; otherwise, <c>false</c>.</returns>
        protected bool IsOfType(TypeInfo x, TypeInfo type)
        {
            if (x == typeof(object).GetTypeInfo() || x == null)
                return false;
            if (x == type || x.ImplementedInterfaces.Any(y => y.GetTypeInfo() == type))
                return true;
            return IsOfType(x.BaseType.GetTypeInfo(), type);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~BootstrapperBase()
        {
            Dispose(false);
        }
    }
}