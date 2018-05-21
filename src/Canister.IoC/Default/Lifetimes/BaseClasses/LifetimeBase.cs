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

using Canister.Default.Lifetimes.Interfaces;
using System;

namespace Canister.Default.Lifetimes.BaseClasses
{
    /// <summary>
    /// Lifetime base class
    /// </summary>
    /// <seealso cref="ILifetime"/>
    public abstract class LifetimeBase : ILifetime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifetimeBase"/> class.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="returnType">Type of the object.</param>
        protected LifetimeBase(Func<IServiceProvider, object> implementation, Type returnType)
        {
            Implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
            ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
        }

        /// <summary>
        /// Gets or sets the implementation.
        /// </summary>
        /// <value>The implementation.</value>
        public Func<IServiceProvider, object> Implementation { get; set; }

        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        /// <value>The type of the object.</value>
        public Type ReturnType { get; }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>A new copy of this life time object</returns>
        public abstract ILifetime Copy();

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>The object to resolve</returns>
        public abstract object Resolve(IServiceProvider provider);
    }
}