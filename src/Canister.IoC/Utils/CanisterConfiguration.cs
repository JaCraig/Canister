using Canister.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Canister.IoC.Utils
{
    /// <summary>
    /// Canister config class
    /// </summary>
    /// <seealso cref="ICanisterConfiguration"/>
    internal class CanisterConfiguration : ICanisterConfiguration
    {
        /// <summary>
        /// Gets or sets the assemblies.
        /// </summary>
        /// <value>The assemblies.</value>
        public List<Assembly> Assemblies { get; set; } = new List<Assembly>();

        /// <summary>
        /// Adds the assembly.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>This</returns>
        public ICanisterConfiguration AddAssembly(params Assembly[] assemblies)
        {
            assemblies ??= Array.Empty<Assembly>();
            Assemblies.AddRange(assemblies.Distinct());
            return this;
        }
    }
}