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

using Canister.Default;
using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Canister
{
    /// <summary>
    /// IoC manager class
    /// </summary>
    public static class Builder
    {
        /// <summary>
        /// Bootstrapper object
        /// </summary>
        public static IBootstrapper? Bootstrapper { get; internal set; }

        /// <summary>
        /// Gets the builder assembly.
        /// </summary>
        /// <value>The builder assembly.</value>
        private static Assembly BuilderAssembly { get; } = typeof(Builder).Assembly;

        /// <summary>
        /// Creates the IoC Container
        /// </summary>
        /// <param name="descriptors">The service descriptors.</param>
        /// <param name="assemblies">The assemblies to scan for modules/types.</param>
        /// <returns>The resulting bootstrapper</returns>
        public static IBootstrapper CreateContainer(IServiceCollection descriptors, params Assembly[] assemblies)
        {
            descriptors ??= new ServiceCollection();
            assemblies ??= Array.Empty<Assembly>();
            var Assemblies = LoadAssemblies(assemblies);
            var LoadedTypes = Assemblies.SelectMany(x => x.ExportedTypes);
            return GetBootstrapper(Assemblies, LoadedTypes, descriptors);
        }

        /// <summary>
        /// Gets the bootstrapper.
        /// </summary>
        /// <param name="Assemblies">The assemblies.</param>
        /// <param name="LoadedTypes">The loaded types.</param>
        /// <param name="descriptors">The descriptors.</param>
        /// <returns>The bootstrapper.</returns>
        private static IBootstrapper GetBootstrapper(IEnumerable<Assembly> Assemblies, IEnumerable<Type> LoadedTypes, IEnumerable<ServiceDescriptor> descriptors)
        {
            var IBootstrapperType = typeof(IBootstrapper);
            var BootstrapperType = LoadedTypes.FirstOrDefault(x => IBootstrapperType.IsAssignableFrom(x)
                                                            && x.IsClass
                                                            && !x.IsAbstract
                                                            && !x.ContainsGenericParameters
                                                            && x.Assembly != BuilderAssembly);
            return (IBootstrapper)Activator.CreateInstance(BootstrapperType ?? typeof(DefaultBootstrapper), Assemblies, descriptors);
        }

        /// <summary>
        /// Loads the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>The loaded assemblies.</returns>
        private static IEnumerable<Assembly> LoadAssemblies(Assembly[] assemblies)
        {
            var Assemblies = new List<Assembly>();
            Assemblies.AddRange(assemblies.Distinct());
            if (Assemblies.Count == 0 || !Assemblies.Contains(BuilderAssembly))
                Assemblies.Add(BuilderAssembly);
            return Assemblies;
        }
    }
}