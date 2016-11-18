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
        public static IBootstrapper Bootstrapper { get; private set; }

        /// <summary>
        /// Creates the IoC Container
        /// </summary>
        /// <param name="descriptors">The service descriptors.</param>
        /// <param name="assemblies">The assemblies to scan for modules/types.</param>
        /// <returns>The resulting bootstrapper</returns>
        public static IBootstrapper CreateContainer(IEnumerable<ServiceDescriptor> descriptors, params Assembly[] assemblies)
        {
            descriptors = descriptors ?? new List<ServiceDescriptor>();
            assemblies = assemblies ?? new Assembly[0];
            var Assemblies = LoadAssemblies(assemblies);
            var LoadedTypes = Assemblies.SelectMany(x => x.ExportedTypes);
            Bootstrapper = GetBootstrapper(Assemblies, LoadedTypes);
            Bootstrapper.Register<IServiceProvider>(Bootstrapper, ServiceLifetime.Singleton);
            RegisterModules();
            RegisterServiceDescriptors(descriptors);
            return Bootstrapper;
        }

        private static IBootstrapper GetBootstrapper(IEnumerable<Assembly> Assemblies, IEnumerable<Type> LoadedTypes)
        {
            var Bootstrappers = LoadedTypes.Where(x => x.GetTypeInfo()
                                                        .ImplementedInterfaces
                                                        .Contains(typeof(IBootstrapper))
                                                            && x.GetTypeInfo().IsClass
                                                            && !x.GetTypeInfo().IsAbstract
                                                            && !x.GetTypeInfo().ContainsGenericParameters
                                                            && !x.GetTypeInfo().Namespace.StartsWith("CANISTER", StringComparison.OrdinalIgnoreCase))
                                                       .ToList();
            if (Bootstrappers.Count == 0)
                Bootstrappers.Add(typeof(DefaultBootstrapper));
            return (IBootstrapper)Activator.CreateInstance(Bootstrappers[0], Assemblies, LoadedTypes);
        }

        private static IEnumerable<Assembly> LoadAssemblies(Assembly[] assemblies)
        {
            var Assemblies = new List<Assembly>();
            Assemblies.AddRange(assemblies);
            if (Assemblies.Count == 0 || !Assemblies.Contains(typeof(Builder).GetTypeInfo().Assembly))
                Assemblies.Add(typeof(Builder).GetTypeInfo().Assembly);
            return Assemblies;
        }

        private static void RegisterModules()
        {
            Bootstrapper.RegisterAll<IModule>();
            foreach (IModule Module in Bootstrapper.ResolveAll<IModule>().OrderBy(x => x.Order))
            {
                Module.Load(Bootstrapper);
            }
        }

        private static void RegisterServiceDescriptors(IEnumerable<ServiceDescriptor> descriptors)
        {
            var RegisterTypes = typeof(IBootstrapper).GetTypeInfo()
                                                                 .GetDeclaredMethods("Register")
                                                                 .First(x => x.GetGenericArguments().Count() == 2);
            var RegisterFunc = typeof(IBootstrapper).GetTypeInfo()
                                                    .GetDeclaredMethods("Register")
                                                     .First(x => x.GetGenericArguments().Count() == 1 &&
                                                            x.GetParameters().Count() == 3 &&
                                                            x.GetParameters()[0].ParameterType.GenericTypeArguments.Length == 2);
            var RegisterObj = typeof(IBootstrapper).GetTypeInfo()
                                                    .GetDeclaredMethods("Register")
                                                     .First(x => x.GetGenericArguments().Count() == 1 &&
                                                            x.GetParameters().Count() == 3 &&
                                                            x.GetParameters()[0].ParameterType.GenericTypeArguments.Length != 2);
            foreach (var item in descriptors)
            {
                if (item.ImplementationType != null)
                {
                    var serviceTypeInfo = item.ServiceType.GetTypeInfo();
                    if (serviceTypeInfo.IsGenericTypeDefinition)
                    {
                        var tempType = serviceTypeInfo.MakeGenericType(item.ServiceType);
                        var tempRegistration = RegisterTypes.MakeGenericMethod(item.ImplementationType, tempType);
                        tempRegistration.Invoke(Bootstrapper, new object[] { item.Lifetime, "" });
                    }
                    else
                    {
                        var tempRegistration = RegisterTypes.MakeGenericMethod(item.ImplementationType, item.ServiceType);
                        tempRegistration.Invoke(Bootstrapper, new object[] { item.Lifetime, "" });
                    }
                }
                else if (item.ImplementationFactory != null)
                {
                    var tempRegistration = RegisterFunc.MakeGenericMethod(item.ImplementationType, item.ServiceType);
                    tempRegistration.Invoke(Bootstrapper, new object[] { item.ImplementationFactory, item.Lifetime, "" });
                }
                else
                {
                    var tempRegistration = RegisterObj.MakeGenericMethod(item.ImplementationType, item.ServiceType);
                    tempRegistration.Invoke(Bootstrapper, new object[] { item.ImplementationInstance, item.Lifetime, "" });
                }
            }
        }
    }
}