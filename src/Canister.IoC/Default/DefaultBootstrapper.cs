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

using Canister.BaseClasses;
using Canister.Default.Services;
using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Canister.Default
{
    /// <summary>
    /// Default bootstrapper if one isn't found
    /// </summary>
    public class DefaultBootstrapper : BootstrapperBase<ServiceTable>, IScope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBootstrapper"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="descriptors">The descriptors.</param>
        public DefaultBootstrapper(IEnumerable<Assembly> assemblies, IEnumerable<ServiceDescriptor> descriptors)
            : base(assemblies)
        {
            _AppContainer = new ServiceTable(descriptors, this);
            Register<IScope>(this);
            Register<IServiceScope>(this);
            Register<DefaultBootstrapper>(this);
            Register<IServiceScopeFactory, ServiceScopeFactory>();
            Register<IScopeFactory, ServiceScopeFactory>();
            Register<ServiceScopeFactory, ServiceScopeFactory>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBootstrapper"/> class.
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper.</param>
        public DefaultBootstrapper(DefaultBootstrapper bootstrapper)
            : base(bootstrapper.Assemblies)
        {
            Parent = bootstrapper;
            _AppContainer = new ServiceTable(bootstrapper._AppContainer);
        }

        /// <summary>
        /// The application container
        /// </summary>
        private ServiceTable _AppContainer;

        /// <summary>
        /// The IoC container
        /// </summary>
        public override ServiceTable AppContainer => _AppContainer;

        /// <summary>
        /// Name of the bootstrapper
        /// </summary>
        public override string Name => "Default bootstrapper";

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <value>The service provider.</value>
        public IServiceProvider ServiceProvider => this;

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        private DefaultBootstrapper Parent { get; set; }

        /// <summary>
        /// Creates a new sub scope.
        /// </summary>
        /// <returns>The new scope</returns>
        public IServiceScope CreateScope()
        {
            return new DefaultBootstrapper(this);
        }

        /// <summary>
        /// Registers an object
        /// </summary>
        /// <typeparam name="T">Type to register</typeparam>
        /// <param name="objectToRegister">The object to register.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public override IBootstrapper Register<T>(T objectToRegister, ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
        {
            AppContainer.Add(typeof(T), name, new InstanceService(typeof(T), objectToRegister, AppContainer, lifeTime));
            return this;
        }

        /// <summary>
        /// Registers an object
        /// </summary>
        /// <typeparam name="T">Type to register</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public override IBootstrapper Register<T>(ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
        {
            if (typeof(T).IsGenericTypeDefinition)
                AppContainer.Add(typeof(T), "", new GenericService(typeof(T), AppContainer, lifeTime));
            else
                AppContainer.Add(typeof(T), name, new ConstructorService(typeof(T), typeof(T), AppContainer, lifeTime));
            return this;
        }

        /// <summary>
        /// Registers two types together
        /// </summary>
        /// <typeparam name="T1">Interface/base class</typeparam>
        /// <typeparam name="T2">Implementation</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public override IBootstrapper Register<T1, T2>(ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
        {
            if (typeof(T1).IsGenericTypeDefinition && typeof(T2).IsGenericTypeDefinition)
                AppContainer.Add(typeof(T1), "", new GenericService(typeof(T2), AppContainer, lifeTime));
            else
                AppContainer.Add(typeof(T1), name, new ConstructorService(typeof(T1), typeof(T2), AppContainer, lifeTime));
            return this;
        }

        /// <summary>
        /// Registers a function with a type
        /// </summary>
        /// <typeparam name="T">Type to register</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public override IBootstrapper Register<T>(Func<IServiceProvider, object> function, ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
        {
            AppContainer.Add(typeof(T), name, new FactoryService(typeof(T), function, AppContainer, lifeTime));
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
            if (objectType.IsGenericTypeDefinition)
                AppContainer.Add(objectType, "", new GenericService(objectType, AppContainer, lifeTime));
            else
                AppContainer.Add(objectType, name, new ConstructorService(objectType, objectType, AppContainer, lifeTime));
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
            foreach (Type TempType in Assemblies.SelectMany(x =>
            {
                try
                {
                    return x.GetTypes();
                }
                catch { return new Type[0]; }
            })
                                                .Where(x => x.IsClass
                                                    && !x.IsAbstract
                                                    && !x.ContainsGenericParameters
                                                    && IsOfType(x, typeof(T))))
            {
                AppContainer.Add(typeof(T), "", new ConstructorService(typeof(T), TempType, AppContainer, lifeTime));
            }
            return this;
        }

        /// <summary>
        /// Resolves an object based on the type specified
        /// </summary>
        /// <typeparam name="T">Type of object to return</typeparam>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>Object of the type specified</returns>
        public override T Resolve<T>(T defaultObject = default(T))
        {
            return (T)Resolve(typeof(T), "", defaultObject);
        }

        /// <summary>
        /// Resolves an object based on the type specified
        /// </summary>
        /// <typeparam name="T">Type of object to return</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>Object of the type specified</returns>
        public override T Resolve<T>(string name, T defaultObject = default(T))
        {
            return (T)Resolve(typeof(T), name, defaultObject);
        }

        /// <summary>
        /// Resolves an object based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>Object of the type specified</returns>
        public override object Resolve(Type objectType, object defaultObject = null)
        {
            return Resolve(objectType, "", defaultObject);
        }

        /// <summary>
        /// Resolves an object based on the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultObject">The default object.</param>
        /// <returns>Object of the type specified</returns>
        public override object Resolve(Type objectType, string name, object defaultObject = null)
        {
            try
            {
                return AppContainer.Resolve(objectType, name);
            }
            catch { return defaultObject; }
        }

        /// <summary>
        /// Resolves all objects of the type specified
        /// </summary>
        /// <typeparam name="T">Type of objects to return</typeparam>
        /// <returns>An IEnumerable containing all objects of the type specified</returns>
        public override IEnumerable<T> ResolveAll<T>()
        {
            return ResolveAll(typeof(T)).Select(x => (T)x).ToList();
        }

        /// <summary>
        /// Resolves all objects of the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>An IEnumerable containing all objects of the type specified</returns>
        public override IEnumerable<object> ResolveAll(Type objectType)
        {
            return AppContainer.GetAllServices(objectType).Select(x => x.Create(this)).ToList();
        }

        /// <summary>
        /// Converts the bootstrapper to a string
        /// </summary>
        /// <returns>String version of the bootstrapper</returns>
        public override string ToString()
        {
            if (_AppContainer == null)
                return "";
            return _AppContainer.ToString();
        }

        /// <summary>
        /// Disposes of the object
        /// </summary>
        /// <param name="managed">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        /// unmanaged resources.
        /// </param>
        protected override void Dispose(bool managed)
        {
            if (_AppContainer != null)
            {
                _AppContainer.Dispose();
                _AppContainer = null;
            }
            base.Dispose(managed);
        }
    }
}