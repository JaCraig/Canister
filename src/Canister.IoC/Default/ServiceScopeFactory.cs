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

using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Canister.Default
{
    /// <summary>
    /// Used to create scopes.
    /// </summary>
    public class ServiceScopeFactory : IScopeFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceScopeFactory"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public ServiceScopeFactory(IScope scope)
        {
            Scope = scope;
        }

        /// <summary>
        /// The scope
        /// </summary>
        private readonly IScope Scope;

        /// <summary>
        /// Creates a new scope object.
        /// </summary>
        /// <returns>The service scope</returns>
        public IServiceScope CreateScope() => Scope.CreateScope();
    }
}