using System;
using System.Collections.Concurrent;

namespace Canister.ExtensionMethods
{
    /// <summary>
    /// Helper extension methods
    /// </summary>
    internal static class HelperFunctions
    {
        /// <summary>
        /// Gets the or add.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TArg">The type of the argument.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="valueFactory">The value factory.</param>
        /// <param name="arg">The argument.</param>
        /// <returns>The value</returns>
        internal static TValue GetOrAdd<TKey, TValue, TArg>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TArg, TValue> valueFactory, TArg arg)
        {
            while (true)
            {
                TValue value;
                if (dictionary.TryGetValue(key, out value))
                {
                    return value;
                }

                value = valueFactory(key, arg);
                if (dictionary.TryAdd(key, value))
                {
                    return value;
                }
            }
        }
    }
}