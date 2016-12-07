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
using System.Collections.Generic;
using System.Linq;

namespace Canister.Default.Services
{
    /// <summary>
    /// Closed IEnumerable Service
    /// </summary>
    /// <seealso cref="ServiceBase"/>
    public class ClosedIEnumerableService : ServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedIEnumerableService"/> class.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="services">The services.</param>
        /// <param name="table">The table.</param>
        /// <param name="lifetime">The lifetime.</param>
        public ClosedIEnumerableService(Type returnType, List<IService> services, ServiceTable table, ServiceLifetime lifetime)
            : base(returnType, table, lifetime)
        {
            Services = services;
        }

        /// <summary>
        /// Gets or sets the services.
        /// </summary>
        /// <value>The services.</value>
        public List<IService> Services { get; set; }

        /// <summary>
        /// Creates the specified service
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>The resulting object</returns>
        protected override object InternalCreate(IServiceProvider provider)
        {
            var TempObjects = Services.Select(x => x.Create(provider)).ToArray();
            var ReturnArray = Array.CreateInstance(ReturnType, TempObjects.Length);
            for (int x = 0; x < TempObjects.Length; ++x)
            {
                ReturnArray.SetValue(TempObjects[x], x);
            }
            return ReturnArray;
        }
    }
}