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
using System.Collections.Concurrent;

namespace Canister.Default.TypeBuilders
{
    /// <summary>
    /// Generic singleton type builder
    /// </summary>
    /// <seealso cref="GenericTypeBuilderBase"/>
    public class GenericSingletonTypeBuilder : GenericTypeBuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericSingletonTypeBuilder"/> class.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="returnType">Type of the return.</param>
        public GenericSingletonTypeBuilder(Func<IServiceProvider, Type[], object> implementation, Type returnType)
            : base(implementation, returnType)
        {
            ResolvedObjects = new ConcurrentDictionary<Type[], object>();
        }

        /// <summary>
        /// Gets or sets the resolved object.
        /// </summary>
        /// <value>The resolved object.</value>
        protected ConcurrentDictionary<Type[], object> ResolvedObjects { get; set; }

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
        /// <returns>The created object</returns>
        public override object Create(IServiceProvider provider)
        {
            object ResolvedObject = null;
            if (!ResolvedObjects.TryGetValue(new Type[0], out ResolvedObject) || Equals(ResolvedObjects, null))
            {
                ResolvedObject = Implementation(provider, new Type[0]);
                ResolvedObjects.AddOrUpdate(new Type[0],
                    x => ResolvedObject,
                    (x, y) => y);
            }
            return ResolvedObject;
        }

        /// <summary>
        /// Creates the object
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="genericParameters">The generic parameters.</param>
        /// <returns>The created object</returns>
        public override object Create(IServiceProvider provider, Type[] genericParameters)
        {
            object ResolvedObject = null;
            if (!ResolvedObjects.TryGetValue(genericParameters, out ResolvedObject) || Equals(ResolvedObjects, null))
            {
                ResolvedObject = Implementation(provider, genericParameters);
                ResolvedObjects.AddOrUpdate(genericParameters,
                    x => ResolvedObject,
                    (x, y) => y);
            }
            return ResolvedObject;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            foreach (var Key in ResolvedObjects.Keys)
            {
                object ResolvedObject = ResolvedObjects[Key];
                var tempValue = ResolvedObject as IDisposable;
                if (tempValue != null)
                {
                    tempValue.Dispose();
                    ResolvedObject = null;
                }
            }
        }
    }
}