﻿/*
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

using Canister.Default.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Canister.Default.Services.BaseClasses
{
    /// <summary>
    /// Generic service base class
    /// </summary>
    /// <seealso cref="IGenericService"/>
    public abstract class GenericServiceBase : IGenericService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericServiceBase"/> class.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="table">The table.</param>
        /// <param name="lifetime">The lifetime.</param>
        protected GenericServiceBase(Type returnType, ServiceTable table, ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
            Table = table;
            ReturnType = returnType;
            LifetimeOfService = lifetime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericServiceBase"/> class.
        /// </summary>
        /// <param name="genericService">The generic service.</param>
        protected GenericServiceBase(GenericServiceBase genericService)
        {
            Lifetime = genericService.Lifetime;
            Table = genericService.Table;
            ReturnType = genericService.ReturnType;
            LifetimeOfService = genericService.LifetimeOfService;
        }

        /// <summary>
        /// Gets or sets the lifetime.
        /// </summary>
        /// <value>The lifetime.</value>
        public ServiceLifetime Lifetime { get; set; }

        /// <summary>
        /// Gets the lifetime of service.
        /// </summary>
        /// <value>The lifetime of service.</value>
        public ServiceLifetime LifetimeOfService { get; set; }

        /// <summary>
        /// Gets the type of the return.
        /// </summary>
        /// <value>The type of the return.</value>
        public Type ReturnType { get; }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>The table.</value>
        public ServiceTable Table { get; set; }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>A copy of this instance</returns>
        public abstract IGenericService Copy();

        /// <summary>
        /// Creates the specified service
        /// </summary>
        /// <param name="closedType">The closed object type</param>
        /// <returns>The resulting object</returns>
        public abstract IService CreateService(Type closedType);
    }
}