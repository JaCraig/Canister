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
using System.Reflection;

namespace Canister.Default.Services
{
    /// <summary>
    /// Generic service class
    /// </summary>
    /// <seealso cref="GenericServiceBase"/>
    public class GenericService : GenericServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericService"/> class.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="table">The table.</param>
        /// <param name="lifetime">The lifetime.</param>
        public GenericService(Type returnType, ServiceTable table, ServiceLifetime lifetime)
            : base(returnType, table, lifetime)
        {
        }

        /// <summary>
        /// Creates the specified service
        /// </summary>
        /// <param name="closedType">The closed object type</param>
        /// <returns>The resulting object</returns>
        public override IService CreateService(Type closedType)
        {
            Type[] GenericArguments = closedType.GetTypeInfo().GenericTypeArguments;
            Type ClosedType = ReturnType.MakeGenericType(GenericArguments);
            return new ConstructorService(ReturnType, ClosedType, Table, Lifetime);
        }
    }
}