using System;
using System.Collections.Generic;
using System.Linq;

namespace Canister.IoC.ExtensionMethods
{
    /// <summary>
    /// Provides extension methods for <see cref="HashSet{T}"/> of <see cref="Type"/> to filter
    /// available classes and interfaces.
    /// </summary>
    public static class HashSetExtensions
    {
        /// <summary>
        /// Returns an array of non-abstract, non-generic classes from the given <see cref="HashSet{Type}"/>.
        /// </summary>
        /// <param name="types">The set of types to filter.</param>
        /// <returns>
        /// An array of <see cref="Type"/> objects that are concrete classes (not abstract, not
        /// generic). Returns an empty array if <paramref name="types"/> is <c>null</c> or empty.
        /// </returns>
        public static Type[] GetAvailableClasses(this HashSet<Type> types)
        {
            if (types is null || types.Count == 0)
                return [];
            return [.. types.Where(x => x.IsClass && !x.IsAbstract && !x.ContainsGenericParameters)];
        }

        /// <summary>
        /// Returns an array of non-generic interfaces from the given <see cref="HashSet{Type}"/>.
        /// </summary>
        /// <param name="types">The set of types to filter.</param>
        /// <returns>
        /// An array of <see cref="Type"/> objects that are interfaces and not generic. Returns an
        /// empty array if <paramref name="types"/> is <c>null</c> or empty.
        /// </returns>
        public static Type[] GetAvailableInterfaces(this HashSet<Type> types)
        {
            if (types is null || types.Count == 0)
                return [];
            return [.. types.Where(x => x.IsInterface && !x.ContainsGenericParameters)];
        }
    }
}