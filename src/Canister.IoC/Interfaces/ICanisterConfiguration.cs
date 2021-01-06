using System.Reflection;

namespace Canister.Interfaces
{
    /// <summary>
    /// Canister configuration interface
    /// </summary>
    public interface ICanisterConfiguration
    {
        /// <summary>
        /// Adds the assembly.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>This</returns>
        ICanisterConfiguration AddAssembly(params Assembly[] assemblies);

        /// <summary>
        /// Builds this instance, loads all modules, etc.
        /// </summary>
        /// <returns>This</returns>
        IBootstrapper Build();
    }
}