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

using Canister.Default.BaseClasses;
using Canister.Default.Interfaces;
using System;

namespace Canister.Default.TypeBuilders
{
    /// <summary>
    /// Singleton type builder
    /// </summary>
    /// <seealso cref="TypeBuilderBase"/>
    public class SingletonTypeBuilder : TypeBuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonTypeBuilder"/> class.
        /// </summary>
        /// <param name="implementation">Implementation</param>
        /// <param name="returnType">Type of the return.</param>
        public SingletonTypeBuilder(Func<IServiceProvider, object> implementation, Type returnType)
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
        /// <returns>A copy of this instance.</returns>
        public override ITypeBuilder Copy()
        {
            return this;
        }

        /// <summary>
        /// Creates the object
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="genericParameters">The generic parameters.</param>
        /// <returns>The created object</returns>
        public override object Create(IServiceProvider provider, Type[] genericParameters)
        {
            if (Equals(ResolvedObject, null))
                ResolvedObject = Implementation(provider);
            return ResolvedObject;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
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
    }
}