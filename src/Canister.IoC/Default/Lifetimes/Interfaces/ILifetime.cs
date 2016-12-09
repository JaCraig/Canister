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

using System;

namespace Canister.Default.Lifetimes.Interfaces
{
    /// <summary>
    /// Lifetime interface
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public interface ILifetime : IDisposable
    {
        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        /// <value>The type of the object.</value>
        Type ReturnType { get; }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>A new copy of this life time object</returns>
        ILifetime Copy();

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>The object to resolve</returns>
        object Resolve(IServiceProvider provider);
    }
}