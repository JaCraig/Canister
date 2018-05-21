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

using Canister.Default.Services.BaseClasses;
using Canister.Default.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Canister.Default.Services
{
    /// <summary>
    /// Constructor service
    /// </summary>
    /// <seealso cref="ServiceBase"/>
    public class ConstructorService : ServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorService"/> class.
        /// </summary>
        /// <param name="constructorService">The constructor service.</param>
        public ConstructorService(ConstructorService constructorService)
            : base(constructorService)
        {
            ImplementationType = constructorService.ImplementationType;
            Constructors = constructorService.Constructors;
            Implementation = constructorService.Implementation;
            Constructor = constructorService.Constructor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorService"/> class.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="table">The table.</param>
        /// <param name="lifetime">The lifetime.</param>
        public ConstructorService(Type returnType, Type implementationType, ServiceTable table, ServiceLifetime lifetime)
            : base(returnType, table, lifetime)
        {
            ImplementationType = implementationType;
            var TempConstructors = ImplementationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).OrderByDescending(x => x.GetParameters().Length).ToArray();
            Constructors = new CachedConstructorInfo[TempConstructors.Length];
            for (int x = 0, TempConstructorsLength = TempConstructors.Length; x < TempConstructorsLength; ++x)
            {
                Constructors[x] = new CachedConstructorInfo
                {
                    Constructor = TempConstructors[x],
                    Parameters = TempConstructors[x].GetParameters()
                };
            }
        }

        /// <summary>
        /// Gets or sets the type of the implementation.
        /// </summary>
        /// <value>The type of the implementation.</value>
        public Type ImplementationType { get; set; }

        /// <summary>
        /// Gets or sets the constructor.
        /// </summary>
        /// <value>The constructor.</value>
        private CachedConstructorInfo Constructor { get; set; }

        /// <summary>
        /// Gets or sets the constructors.
        /// </summary>
        /// <value>The constructors.</value>
        private CachedConstructorInfo[] Constructors { get; }

        /// <summary>
        /// Gets or sets the implementation.
        /// </summary>
        /// <value>The implementation.</value>
        private Func<IServiceProvider, object> Implementation { get; set; }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>A copy of this instance</returns>
        public override IService Copy()
        {
            return new ConstructorService(this);
        }

        /// <summary>
        /// Creates the specified service
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>The resulting object</returns>
        protected override object InternalCreate(IServiceProvider provider)
        {
            if (Constructor is null)
                Constructor = FindConstructor();

            if (Implementation is null && LifetimeOfService != ServiceLifetime.Singleton)
                Implementation = CreateFunc(Constructor);

            if (!(Implementation is null))
                return Implementation(provider);

            return Constructor?.Constructor.Invoke(GetParameters());
        }

        /// <summary>
        /// Creates the function.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns></returns>
        private Func<IServiceProvider, object> CreateFunc(CachedConstructorInfo constructor)
        {
            if (Constructor is null) return null;
            ParameterExpression ServiceProviderParameter = Expression.Parameter(typeof(IServiceProvider));
            Expression[] parameters = new Expression[constructor.Parameters.Length];
            MethodInfo ServiceMethod = typeof(IServiceProvider).GetMethod("GetService");
            for (int x = 0, parametersLength = parameters.Length; x < parametersLength; ++x)
            {
                var ExpressionConstant = Expression.Constant(constructor.Parameters[x].ParameterType, typeof(Type));
                parameters[x] = Expression.Convert(Expression.Call(ServiceProviderParameter, ServiceMethod, ExpressionConstant), constructor.Parameters[x].ParameterType);
            }
            NewExpression newExpression = Expression.New(constructor.Constructor, parameters);
            LambdaExpression lambda = Expression.Lambda<Func<IServiceProvider, object>>(newExpression, ServiceProviderParameter);
            return (Func<IServiceProvider, object>)lambda.Compile();
        }

        /// <summary>
        /// Finds the constructor.
        /// </summary>
        /// <returns>The appropriate construtor</returns>
        private CachedConstructorInfo FindConstructor()
        {
            for (int x = 0, ConstructorsLength = Constructors.Length; x < ConstructorsLength; x++)
            {
                var TempConstructor = Constructors[x];
                bool Found = true;
                var Parameters = TempConstructor.Parameters;
                for (int y = 0, maxLength = Parameters.Length; y < maxLength; y++)
                {
                    var Parameter = Parameters[y];
                    var TempServices = Table.GetServices(Parameter.ParameterType);
                    if (TempServices.Count == 0 && !Parameter.IsOptional)
                    {
                        Found = false;
                        break;
                    }
                }

                if (Found)
                    return TempConstructor;
            }

            return null;
        }

        /// <summary>
        /// Gets the parameters if using the constructor.
        /// </summary>
        /// <returns>Called to get the parameters.</returns>
        private object[] GetParameters()
        {
            var Parameters = Constructor.Parameters;
            var ReturnObject = new object[Parameters.Length];
            for (int x = 0, ParametersLength = Parameters.Length; x < ParametersLength; ++x)
            {
                var TempParameter = Parameters[x];
                ReturnObject[x] = Table.Resolve(TempParameter.ParameterType, "", LifetimeOfService);
                if (ReturnObject[x] is null && TempParameter.IsOptional)
                    ReturnObject[x] = TempParameter.DefaultValue;
            }
            return ReturnObject;
        }

        /// <summary>
        /// Caches the constructor info.
        /// </summary>
        private class CachedConstructorInfo
        {
            /// <summary>
            /// Gets or sets the constructor.
            /// </summary>
            /// <value>The constructor.</value>
            public ConstructorInfo Constructor { get; set; }

            /// <summary>
            /// Gets or sets the parameters.
            /// </summary>
            /// <value>The parameters.</value>
            public ParameterInfo[] Parameters { get; set; }
        }
    }
}