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

*/using Canister.Default.Lifetimes.BaseClasses;
using Canister.Default.Lifetimes.Interfaces;
using System;

namespace Canister.Default.Lifetimes
{
    /// <summary>
    /// Singleton life time
    /// </summary>
    /// <seealso cref="LifetimeBase"/>
    public class SingletonLifetime : LifetimeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonLifetime"/> class.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="returnType">Type of the return.</param>
        public SingletonLifetime(Func<IServiceProvider, object> implementation, Type returnType)
            : base(implementation, returnType)
        {
        }

        /// <summary>
        /// Gets or sets the resolved object.
        /// </summary>
        /// <value>The resolved object.</value>
        protected object ResolvedObject { get; set; }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>A new copy of this life time object</returns>
        public override ILifetime Copy()
        {
            return this;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public override void Dispose()
        {
            var tempValue = ResolvedObject as IDisposable;
            if (tempValue != null)
            {
                tempValue.Dispose();
                ResolvedObject = null;
            }
        }

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>The object to resolve</returns>
        public override object Resolve(IServiceProvider provider)
        {
            if (Equals(ResolvedObject, null))
                ResolvedObject = Implementation(provider);
            return ResolvedObject;
        }
    }
}