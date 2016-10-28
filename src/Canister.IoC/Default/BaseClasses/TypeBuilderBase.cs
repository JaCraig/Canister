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

using Canister.Default.Interfaces;
using System;

namespace Canister.Default.BaseClasses
{
    /// <summary>
    /// Type builder base class
    /// </summary>
    /// <typeparam name="T">The type of the object it creates</typeparam>
    public abstract class TypeBuilderBase<T> : ITypeBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeBuilderBase{T}"/> class.
        /// </summary>
        /// <param name="implementation">Implementation</param>
        protected TypeBuilderBase(Func<IServiceProvider, T> implementation)
        {
            Implementation = implementation ?? new Func<IServiceProvider, T>(x => (T)Activator.CreateInstance(typeof(T)));
            ReturnType = typeof(T);
        }

        /// <summary>
        /// Return type of the implementation
        /// </summary>
        public Type ReturnType { get; private set; }

        /// <summary>
        /// Implementation used to create the type
        /// </summary>
        protected Func<IServiceProvider, T> Implementation { get; private set; }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        public abstract ITypeBuilder Copy();

        /// <summary>
        /// Creates the object
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>The object</returns>
        public abstract object Create(IServiceProvider provider);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Outputs the string version of whatever object the builder holds
        /// </summary>
        /// <returns>The string version of the object this holds</returns>
        public override string ToString()
        {
            return ReturnType.Name;
        }
    }
}