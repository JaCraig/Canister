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

using Microsoft.Extensions.DependencyInjection;

namespace Canister.Interfaces
{
    /// <summary>
    /// Represents a module that can be loaded into the IoC container. Implementations define
    /// service registrations and their order.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Gets the order in which this module should be loaded. Modules with lower order values
        /// are loaded first.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Loads the module and registers its services with the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceDescriptors">
        /// The <see cref="IServiceCollection"/> to which services should be registered.
        /// </param>
        void Load(IServiceCollection serviceDescriptors);
    }
}