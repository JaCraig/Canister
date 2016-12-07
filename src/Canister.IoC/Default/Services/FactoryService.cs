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
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Canister.Default.Services
{
    /// <summary>
    /// Factory service
    /// </summary>
    /// <seealso cref="ServiceBase"/>
    public class FactoryService : ServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryService"/> class.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="table">The table.</param>
        /// <param name="lifetime">The lifetime.</param>
        public FactoryService(Type returnType, Func<IServiceProvider, object> implementation, ServiceTable table, ServiceLifetime lifetime)
            : base(returnType, table, lifetime)
        {
            Implementation = implementation;
        }

        /// <summary>
        /// Gets or sets the implementation.
        /// </summary>
        /// <value>The implementation.</value>
        public Func<IServiceProvider, object> Implementation { get; set; }

        /// <summary>
        /// Creates the specified service
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>The resulting object</returns>
        protected override object InternalCreate(IServiceProvider provider)
        {
            return Implementation(provider);
        }
    }
}