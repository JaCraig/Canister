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
    /// Open IEnumerable Service
    /// </summary>
    /// <seealso cref="ServiceBase"/>
    public class OpenIEnumerableService : GenericServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIEnumerableService"/> class.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="table">The table.</param>
        /// <param name="lifetime">The lifetime.</param>
        public OpenIEnumerableService(Type returnType, ServiceTable table, ServiceLifetime lifetime)
            : base(returnType, table, lifetime)
        {
            Table = table;
        }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>The table.</value>
        public ServiceTable Table { get; set; }

        /// <summary>
        /// Creates the specified service
        /// </summary>
        /// <param name="closedType">The closed object type</param>
        /// <returns>The resulting object</returns>
        public override IService CreateService(Type closedType)
        {
            Type ObjectType = closedType.GetTypeInfo().GenericTypeArguments[0];
            var TempServices = Table.GetServices(ObjectType);
            return TempServices.Count > 0 ? new ClosedIEnumerableService(ObjectType, TempServices, Table, Lifetime) : null;
        }
    }
}