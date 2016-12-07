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
    /// Generic type builder base class
    /// </summary>
    /// <seealso cref="IGenericTypeBuilder"/>
    public abstract class GenericTypeBuilderBase : IGenericTypeBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericTypeBuilderBase"/> class.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="returnType">Type of the return.</param>
        protected GenericTypeBuilderBase(Func<IServiceProvider, Type[], object> implementation, Type returnType)
        {
            Implementation = implementation ?? new Func<IServiceProvider, Type[], object>((x, y) =>
            {
                var TempType = returnType.MakeGenericType(y);
                return Activator.CreateInstance(TempType);
            });
            ReturnType = returnType;
        }

        /// <summary>
        /// Return type of the implementation
        /// </summary>
        public Type ReturnType { get; private set; }

        /// <summary>
        /// Implementation used to create the type
        /// </summary>
        protected Func<IServiceProvider, Type[], object> Implementation { get; private set; }

        public bool CanResolve(Type objectType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        public abstract ITypeBuilder Copy();

        /// <summary>
        /// Creates the object
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="genericParameters">The generic parameters.</param>
        /// <returns>The object</returns>
        public abstract object Create(IServiceProvider provider, Type[] genericParameters);

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