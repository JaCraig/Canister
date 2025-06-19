using Canister.Interfaces;
using Canister.IoC.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Canister.IoC.Utils
{
    /// <summary>
    /// Provides configuration management for the Canister IoC container.
    /// </summary>
    /// <seealso cref="ICanisterConfiguration"/>
    /// <remarks>
    /// This class is responsible for managing the configuration of the Canister IoC container. It
    /// handles the collection of assemblies and types that are available for dependency injection,
    /// and provides methods to add assemblies and retrieve available types and interfaces.
    /// </remarks>
    internal class CanisterConfiguration : ICanisterConfiguration
    {
        /// <summary>
        /// Stores the set of available interfaces discovered from the configured assemblies.
        /// </summary>
        private HashSet<Type>? _AvailableInterfaces;

        /// <summary>
        /// Stores the set of available concrete types discovered from the configured assemblies.
        /// </summary>
        private HashSet<Type>? _AvailableTypes;

        /// <summary>
        /// Stores the set of all types discovered from the configured assemblies.
        /// </summary>
        private HashSet<Type>? _Types;

        /// <summary>
        /// Gets or sets the list of assemblies used for dependency injection.
        /// </summary>
        /// <value>A set of <see cref="Assembly"/> instances to be used by the IoC container.</value>
        public HashSet<Assembly> Assemblies { get; set; } = [];

        /// <summary>
        /// Gets the set of available interfaces from the configured assemblies.
        /// </summary>
        /// <value>A set of <see cref="Type"/> objects representing interfaces.</value>
        public HashSet<Type> AvailableInterfaces
        {
            get
            {
                if (_AvailableInterfaces is not null)
                    return _AvailableInterfaces;
                lock (AvailableInterfacesLockObject)
                {
                    if (_AvailableInterfaces is not null)
                        return _AvailableInterfaces;
                    _AvailableInterfaces = [.. Types.GetAvailableInterfaces()];
                    return _AvailableInterfaces;
                }
            }
        }

        /// <summary>
        /// Gets the set of available concrete types (classes) from the configured assemblies.
        /// </summary>
        /// <value>A set of <see cref="Type"/> objects representing concrete classes.</value>
        public HashSet<Type> AvailableTypes
        {
            get
            {
                if (_AvailableTypes is not null)
                    return _AvailableTypes;
                lock (AvailableTypesLockObject)
                {
                    if (_AvailableTypes is not null)
                        return _AvailableTypes;
                    _AvailableTypes = [.. Types.GetAvailableClasses()];
                    return _AvailableTypes;
                }
            }
        }

        /// <summary>
        /// Gets the set of all types from the configured assemblies, ensuring uniqueness.
        /// </summary>
        /// <value>A set of <see cref="Type"/> objects found in the configured assemblies.</value>
        public HashSet<Type> Types
        {
            get
            {
                Assemblies ??= [];
                if (Assemblies.Count == 0)
                {
                    AddDefaultAssemblies();
                }
                if (_Types is not null)
                    return _Types;
                lock (TypeLockObject)
                {
                    if (_Types is not null)
                        return _Types;
                    _Types = [.. Assemblies.SelectMany(x => { try { return x.GetTypes(); } catch (ReflectionTypeLoadException) { return []; } })];
                }
                return _Types;
            }
        }

        /// <summary>
        /// Lock object used to synchronize access to the <see cref="Assemblies"/> property.
        /// </summary>
        private object AssemblyLockObject { get; } = new();

        /// <summary>
        /// Lock object used to synchronize access to the <see cref="AvailableInterfaces"/> property.
        /// </summary>
        private object AvailableInterfacesLockObject { get; } = new();

        /// <summary>
        /// Lock object used to synchronize access to the <see cref="AvailableTypes"/> property.
        /// </summary>
        private object AvailableTypesLockObject { get; } = new();

        /// <summary>
        /// Gets the lock object used to synchronize access to the <see cref="Types"/> property.
        /// </summary>
        private object TypeLockObject { get; } = new();

        /// <summary>
        /// Adds one or more assemblies to the configuration.
        /// </summary>
        /// <param name="assemblies">An array of <see cref="Assembly"/> objects to add.</param>
        /// <returns>The current <see cref="ICanisterConfiguration"/> instance for method chaining.</returns>
        public ICanisterConfiguration AddAssembly(params Assembly[] assemblies)
        {
            if (assemblies is null || assemblies.Length == 0)
                return this;
            lock (AssemblyLockObject)
            {
                foreach (var Assembly in assemblies)
                {
                    if (Assembly is null)
                        continue;
                    Assemblies.Add(Assembly);
                }
            }
            return this;
        }

        /// <summary>
        /// Adds a collection of assemblies to the configuration.
        /// </summary>
        /// <param name="assemblies">
        /// An <see cref="IEnumerable{Assembly}"/> containing assemblies to add.
        /// </param>
        /// <returns>The current <see cref="ICanisterConfiguration"/> instance for method chaining.</returns>
        public ICanisterConfiguration AddAssembly(IEnumerable<Assembly> assemblies)
        {
            if (assemblies?.Any() != true)
                return this;
            lock (AssemblyLockObject)
            {
                foreach (var Assembly in assemblies)
                {
                    if (Assembly is null)
                        continue;
                    Assemblies.Add(Assembly);
                }
            }
            return this;
        }

        /// <summary>
        /// Adds the entry and executing assemblies, as well as all assemblies in their directories,
        /// to the configuration.
        /// </summary>
        /// <returns>The current <see cref="ICanisterConfiguration"/> instance for method chaining.</returns>
        public ICanisterConfiguration AddDefaultAssemblies()
        {
            lock (AssemblyLockObject)
            {
                var EntryAssembly = Assembly.GetEntryAssembly();
                if (EntryAssembly is null)
                    return this;

                var ExecutingAssembly = Assembly.GetExecutingAssembly();
                if (ExecutingAssembly is null)
                    return this;

                Assemblies.Add(EntryAssembly);
                Assemblies.Add(ExecutingAssembly);

                var PathsFound = new HashSet<string>
                {
                    Path.GetDirectoryName(EntryAssembly.Location) ?? "",
                    Path.GetDirectoryName(ExecutingAssembly.Location) ?? ""
                };

                foreach (var Path in PathsFound)
                {
                    foreach (var AssemblyFile in Directory.EnumerateFiles(Path, "*.dll", SearchOption.TopDirectoryOnly))
                    {
                        try
                        {
                            Assemblies.Add(Assembly.LoadFrom(AssemblyFile));
                        }
                        catch
                        {
                        }
                    }
                }
                return this;
            }
        }
    }
}