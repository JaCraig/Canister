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

using Canister.Default.Lifetimes;
using Canister.Default.Lifetimes.Interfaces;
using Canister.Default.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Canister.Default.Services.BaseClasses
{
    /// <summary>
    /// Service base class
    /// </summary>
    /// <seealso cref="IService"/>
    public abstract class ServiceBase : IService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase"/> class.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="table">The table.</param>
        /// <param name="lifetime">The lifetime.</param>
        protected ServiceBase(Type returnType, ServiceTable table, ServiceLifetime lifetime)
        {
            Table = table;
            ReturnType = returnType;
            if (lifetime == ServiceLifetime.Scoped)
                Lifetime = new ScopedLifetime(InternalCreate, ReturnType);
            else if (lifetime == ServiceLifetime.Singleton)
                Lifetime = new SingletonLifetime(InternalCreate, ReturnType);
            else
                Lifetime = new TransientLifetime(InternalCreate, ReturnType);
        }

        /// <summary>
        /// Gets or sets the lifetime.
        /// </summary>
        /// <value>The lifetime.</value>
        public ILifetime Lifetime { get; set; }

        /// <summary>
        /// Gets the type of the return.
        /// </summary>
        /// <value>The type of the return.</value>
        public Type ReturnType { get; private set; }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>The table.</value>
        public ServiceTable Table { get; set; }

        /// <summary>
        /// Creates the specified service
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>The resulting object</returns>
        public object Create(IServiceProvider provider)
        {
            return Lifetime.Resolve(provider);
        }

        protected abstract object InternalCreate(IServiceProvider provider);
    }
}