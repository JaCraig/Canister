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

namespace Canister.Default.Interfaces
{
    /// <summary>
    /// Type builder interface
    /// </summary>
    public interface ITypeBuilder : IDisposable
    {
        /// <summary>
        /// Return type of the builder
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        ITypeBuilder Copy();

        /// <summary>
        /// Creates the object
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>The object</returns>
        object Create(IServiceProvider provider);
    }
}