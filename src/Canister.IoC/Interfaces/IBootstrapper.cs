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

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Canister.Interfaces
{
    /// <summary>
    /// Bootstrapper interface
    /// </summary>
    public interface IBootstrapper : IDisposable, IServiceProvider
    {
        /// <summary>
        /// Name of the bootstrapper
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Adds the assembly.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        IBootstrapper AddAssembly(params Assembly[] assemblies);

        /// <summary>
        /// Registers an object with the bootstrapper
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="objectToRegister">The object to register.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        IBootstrapper Register<T>(T objectToRegister, ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
            where T : class;

        /// <summary>
        /// Registers a type with the default constructor
        /// </summary>
        /// <typeparam name="T">Object type to register</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        IBootstrapper Register<T>(ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
            where T : class;

        /// <summary>
        /// Registers a type with the default constructor of a child class
        /// </summary>
        /// <typeparam name="T1">Base class/interface type</typeparam>
        /// <typeparam name="T2">Child class type</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        IBootstrapper Register<T1, T2>(ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
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
        IBootstrapper Register<T>(Func<IServiceProvider, object> function, ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
            where T : class;

        /// <summary>
        /// Registers all objects of a certain type with the bootstrapper
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <returns>This</returns>
        IBootstrapper RegisterAll<T>(ServiceLifetime lifeTime = ServiceLifetime.Transient)
            where T : class;

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        T Resolve<T>(T defaultObject = default(T))
            where T : class;

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        T Resolve<T>(string name, T defaultObject = default(T))
            where T : class;

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        object Resolve(Type objectType, object defaultObject = null);

        /// <summary>
        /// Resolves the object based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>An object of the specified type</returns>
        object Resolve(Type objectType, string name, object defaultObject = null);

        /// <summary>
        /// Resolves the objects based on the type specified
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>A list of objects of the specified type</returns>
        IEnumerable<T> ResolveAll<T>()
            where T : class;

        /// <summary>
        /// Resolves all objects based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A list of objects of the specified type</returns>
        IEnumerable<object> ResolveAll(Type objectType);
    }
}