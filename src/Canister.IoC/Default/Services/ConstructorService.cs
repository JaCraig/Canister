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
        }

        /// <summary>
        /// Gets or sets the constructor.
        /// </summary>
        /// <value>The constructor.</value>
        public ConstructorInfo Constructor { get; set; }

        /// <summary>
        /// Gets or sets the type of the implementation.
        /// </summary>
        /// <value>The type of the implementation.</value>
        public Type ImplementationType { get; set; }

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
            if (Constructor == null)
                Constructor = FindConstructor();
            if (Constructor != null)
            {
                return Constructor.Invoke(GetParameters());
            }
            return null;
        }

        private ConstructorInfo FindConstructor()
        {
            var Constructors = ImplementationType.GetTypeInfo().DeclaredConstructors.Where(x => x.IsPublic).OrderByDescending(x => x.GetParameters().Length);
            foreach (var TempConstructor in Constructors)
            {
                bool Found = true;
                foreach (var Parameter in TempConstructor.GetParameters())
                {
                    var TempServices = Table.GetServices(Parameter.ParameterType);
                    if (TempServices.Count == 0)
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

        private object[] GetParameters()
        {
            var Parameters = Constructor.GetParameters();
            var ReturnObject = new object[Parameters.Length];
            for (int x = 0; x < Parameters.Length; ++x)
            {
                ReturnObject[x] = Table.Resolve(Parameters[x].ParameterType);
            }
            return ReturnObject;
        }
    }
}