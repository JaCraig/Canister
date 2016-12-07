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
using Canister.Default.Interfaces;
using Canister.Default.TypeBuilders;
using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Canister.Default
{
    /// <summary>
    /// Default bootstrapper if one isn't found
    /// </summary>
    public class DefaultBootstrapper : BootstrapperBase<IDictionary<Tuple<Type, string>, ITypeBuilder>>, IScope
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="types">The types.</param>
        public DefaultBootstrapper(IEnumerable<Assembly> assemblies, IEnumerable<Type> types)
            : base(assemblies, types)
        {
            _AppContainer = new ConcurrentDictionary<Tuple<Type, string>, ITypeBuilder>();
            if (GenericRegisterMethod == null)
            {
                GenericRegisterMethod = GetType().GetTypeInfo().DeclaredMethods.First(x => x.Name == "Register" && x.GetGenericArguments().Count() == 2);
                GenericResolveMethod = GetType().GetTypeInfo().DeclaredMethods.First(x => x.Name == "Resolve" && x.IsGenericMethod && x.GetParameters().Length == 1);
                GenericResolveAllMethod = GetType().GetTypeInfo().DeclaredMethods.First(x => x.Name == "ResolveAll" && x.GetParameters().Length == 0);
                Register<IScope, DefaultBootstrapper>();
                Register<IServiceScope, DefaultBootstrapper>();
                Register<DefaultBootstrapper, DefaultBootstrapper>();
                Register<IServiceScopeFactory, ServiceScopeFactory>();
                Register<IScopeFactory, ServiceScopeFactory>();
                Register<ServiceScopeFactory, ServiceScopeFactory>();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBootstrapper"/> class.
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper.</param>
        public DefaultBootstrapper(DefaultBootstrapper bootstrapper)
            : this(bootstrapper.Assemblies, bootstrapper.Types)
        {
            Parent = bootstrapper;
            foreach (var Key in bootstrapper.AppContainer)
            {
                _AppContainer.AddOrUpdate(Key.Key,
                                x => Key.Value.Copy(),
                                (x, y) => Key.Value.Copy());
            }
        }

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
        /// App container
        /// </summary>
        protected override IDictionary<Tuple<Type, string>, ITypeBuilder> AppContainer => _AppContainer;

        /// <summary>
        /// Gets or sets the generic register method.
        /// </summary>
        /// <value>The generic register method.</value>
        private static MethodInfo GenericRegisterMethod { get; set; }

        /// <summary>
        /// Gets or sets the generic resolve all method.
        /// </summary>
        /// <value>The generic resolve all method.</value>
        private static MethodInfo GenericResolveAllMethod { get; set; }

        /// <summary>
        /// Gets or sets the generic resolve method.
        /// </summary>
        /// <value>The generic resolve method.</value>
        private static MethodInfo GenericResolveMethod { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        private DefaultBootstrapper Parent { get; set; }

        /// <summary>
        /// The application container
        /// </summary>
        private ConcurrentDictionary<Tuple<Type, string>, ITypeBuilder> _AppContainer;

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
            return Register<T>(x => objectToRegister, lifeTime, name);
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
            return Register<T, T>(lifeTime, name);
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
            Type Type = typeof(T2);
            return Register<T1>(x =>
            {
                var Constructor = FindConstructor(Type);
                if (Constructor != null)
                {
                    var TempParameters = GetParameters(Constructor).ToArray();
                    return (T1)Activator.CreateInstance(Type, TempParameters);
                }
                return default(T1);
            }, lifeTime, name);
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
            var Key = new Tuple<Type, string>(typeof(T), name);
            var Value = GetTypeBuilder(function, lifeTime, typeof(T));
            _AppContainer.AddOrUpdate(Key,
                x => Value,
                (x, y) => Value);
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
            foreach (Type TempType in Types.Where(x => IsOfType(x, typeof(T))
                                                    && x.GetTypeInfo().IsClass
                                                    && !x.GetTypeInfo().IsAbstract
                                                    && !x.GetTypeInfo().ContainsGenericParameters))
            {
                GenericRegisterMethod.MakeGenericMethod(typeof(T), TempType)
                    .Invoke(this, new object[] { lifeTime, Types.Count == 1 ? "" : TempType.FullName });
                GenericRegisterMethod.MakeGenericMethod(TempType, TempType)
                    .Invoke(this, new object[] { lifeTime, "" });
            }
            return this;
        }

        /// <summary>
        /// Registers a generic with the default constructor
        /// </summary>
        /// <typeparam name="T">Object type to register</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public override IBootstrapper RegisterGeneric<T>(ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
        {
            return RegisterGeneric<T, T>(lifeTime, name);
        }

        /// <summary>
        /// Registers a generic type with the default constructor of a child class
        /// </summary>
        /// <typeparam name="T1">Base class/interface type</typeparam>
        /// <typeparam name="T2">Child class type</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public override IBootstrapper RegisterGeneric<T1, T2>(ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
        {
            Type ImplementationType = typeof(T2);
            return RegisterGeneric<T1>((x, y) =>
            {
                var TempType = ImplementationType.MakeGenericType(y);
                var Constructor = FindConstructor(TempType);
                if (Constructor != null)
                {
                    return (T1)Activator.CreateInstance(TempType, GetParameters(Constructor).ToArray());
                }
                return default(T1);
            }, lifeTime, name);
        }

        /// <summary>
        /// Registers a generic type with a function
        /// </summary>
        /// <typeparam name="T">Type that the function returns</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public override IBootstrapper RegisterGeneric<T>(Func<IServiceProvider, Type[], object> function, ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
        {
            var Key = new Tuple<Type, string>(typeof(T), name);
            var Value = GetTypeBuilder(function, lifeTime, typeof(T));
            _AppContainer.AddOrUpdate(Key,
                x => Value,
                (x, y) => Value);
            return this;
        }

        /// <summary>
        /// Registers a generic type with the default constructor of a child class
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="name">The name.</param>
        /// <returns>This</returns>
        public override IBootstrapper RegisterGeneric(Type service, Type implementation, ServiceLifetime lifeTime = ServiceLifetime.Transient, string name = "")
        {
            var Key = new Tuple<Type, string>(service, name);
            var Value = GetTypeBuilder((x, y) =>
            {
                var TempType = implementation.MakeGenericType(y);
                var Constructor = FindConstructor(TempType);
                if (Constructor != null)
                {
                    return Activator.CreateInstance(TempType, GetParameters(Constructor).ToArray());
                }
                return null;
            }, lifeTime, service);
            _AppContainer.AddOrUpdate(Key,
                x => Value,
                (x, y) => Value);
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
                var Key = new Tuple<Type, string>(objectType, name);
                ITypeBuilder Builder = null;
                if (!_AppContainer.TryGetValue(Key, out Builder))
                {
                    if (objectType.GenericTypeArguments.Length > 0)
                    {
                        var GenericObjectType = objectType.GetGenericTypeDefinition();
                        Key = new Tuple<Type, string>(GenericObjectType, name);
                        if (_AppContainer.TryGetValue(Key, out Builder))
                        {
                            return Builder.Create(this, objectType.GenericTypeArguments);
                        }
                    }
                    return defaultObject;
                }
                return objectType.GenericTypeArguments.Length > 0 ?
                    Builder.Create(this, objectType.GenericTypeArguments) :
                    Builder.Create(this, new Type[0]);
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
            return ResolveAll(typeof(T)).Select(x => (T)x);
        }

        /// <summary>
        /// Resolves all objects of the type specified
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>An IEnumerable containing all objects of the type specified</returns>
        public override IEnumerable<object> ResolveAll(Type objectType)
        {
            var ReturnValues = new ConcurrentBag<object>();
            foreach (Tuple<Type, string> Key in _AppContainer.Keys.Where(x => x.Item1 == objectType))
            {
                ReturnValues.Add(Resolve(Key.Item1, Key.Item2, null));
            }
            return ReturnValues;
        }

        /// <summary>
        /// Converts the bootstrapper to a string
        /// </summary>
        /// <returns>String version of the bootstrapper</returns>
        public override string ToString()
        {
            if (_AppContainer == null)
                return "";
            var Builder = new StringBuilder();
            foreach (Tuple<Type, string> Key in _AppContainer.Keys)
            {
                var TypeBuilder = _AppContainer[Key];
                if (TypeBuilder != null)
                {
                    Builder.AppendLine(TypeBuilder.ToString());
                }
            }
            return Builder.ToString();
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
                IEnumerable<ITypeBuilder> ItemsToDispose = null;
                if (Parent == null)
                    ItemsToDispose = _AppContainer.Values.Reverse();
                else
                    ItemsToDispose = _AppContainer.Values.Where(x => x as IScopedTypeBuilder != null).Reverse();
                foreach (IDisposable Item in ItemsToDispose)
                {
                    Item.Dispose();
                }
                _AppContainer.Clear();
                _AppContainer = null;
            }
            base.Dispose(managed);
        }

        /// <summary>
        /// Gets the type builder.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <returns>The type builder based on the lifetime value.</returns>
        private static ITypeBuilder GetTypeBuilder(Func<IServiceProvider, object> function, ServiceLifetime lifeTime, Type returnType)
        {
            if (lifeTime == ServiceLifetime.Transient)
                return new TransientTypeBuilder(function, returnType);
            if (lifeTime == ServiceLifetime.Scoped)
                return new ScopedTypeBuilder(function, returnType);
            return new SingletonTypeBuilder(function, returnType);
        }

        /// <summary>
        /// Gets the type builder.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <returns>The type builder based on the lifetime value.</returns>
        private static ITypeBuilder GetTypeBuilder(Func<IServiceProvider, Type[], object> function, ServiceLifetime lifeTime, Type returnType)
        {
            if (lifeTime == ServiceLifetime.Transient)
                return new GenericTransientTypeBuilder(function, returnType);
            if (lifeTime == ServiceLifetime.Scoped)
                return new GenericScopedTypeBuilder(function, returnType);
            return new GenericSingletonTypeBuilder(function, returnType);
        }

        /// <summary>
        /// Finds the constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The constructor that should be used</returns>
        private ConstructorInfo FindConstructor(Type type)
        {
            if (type == null)
                return null;
            var Constructors = type.GetTypeInfo().DeclaredConstructors;
            ConstructorInfo Constructor = null;
            foreach (ConstructorInfo TempConstructor in Constructors.OrderByDescending(x => x.GetParameters().Length))
            {
                bool Found = true;
                foreach (ParameterInfo Parameter in TempConstructor.GetParameters())
                {
                    Type ParameterType = Parameter.ParameterType;
                    if (Parameter.ParameterType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEnumerable)) && Parameter.ParameterType.GetTypeInfo().IsGenericType)
                    {
                        ParameterType = ParameterType.GetTypeInfo().GenericTypeArguments.First();
                        if (!AppContainer.Keys.Any(x => x.Item1 == ParameterType)
                            && !AppContainer.Keys.Any(x => x.Item1 == ParameterType.GetGenericTypeDefinition()))
                        {
                            Found = false;
                            break;
                        }
                    }
                    else if (ParameterType.GenericTypeArguments.Length > 0)
                    {
                        if (!AppContainer.Keys.Contains(new Tuple<Type, string>(ParameterType.GetGenericTypeDefinition(), ""))
                        && !AppContainer.Keys.Contains(new Tuple<Type, string>(ParameterType, "")))
                        {
                            Found = false;
                            break;
                        }
                    }
                    else
                    {
                        if (!AppContainer.Keys.Contains(new Tuple<Type, string>(ParameterType, "")))
                        {
                            Found = false;
                            break;
                        }
                    }
                }
                if (Found)
                {
                    Constructor = TempConstructor;
                    break;
                }
            }
            return Constructor;
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The parameters</returns>
        private List<object> GetParameters(ConstructorInfo constructor)
        {
            if (constructor == null)
                return new List<object>();
            var Params = new List<object>();
            foreach (ParameterInfo Parameter in constructor.GetParameters())
            {
                Type TypeToCreate = Parameter.ParameterType;
                if (TypeToCreate.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEnumerable)) && TypeToCreate.GetTypeInfo().IsGenericType)
                {
                    TypeToCreate = TypeToCreate.GetTypeInfo().GenericTypeArguments.First();
                    if (TypeToCreate.GenericTypeArguments.Length > 0
                        && AppContainer.Keys.Any(x => x.Item1 == TypeToCreate.GetGenericTypeDefinition()))
                    {
                        TypeToCreate = TypeToCreate.GetGenericTypeDefinition();
                    }

                    Params.Add(GenericResolveAllMethod.MakeGenericMethod(TypeToCreate).Invoke(this, new object[] { }));
                }
                else
                {
                    Params.Add(Resolve(TypeToCreate, (object)null));
                }
            }
            return Params;
        }

        private bool IsOfType(Type x, Type type)
        {
            if (x == typeof(object) || x == null)
                return false;
            if (x == type || x.GetTypeInfo().ImplementedInterfaces.Any(y => y == type))
                return true;
            return IsOfType(x.GetTypeInfo().BaseType, type);
        }
    }
}