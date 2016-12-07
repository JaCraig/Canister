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

using Canister.Default.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Canister.Default.Services
{
    /// <summary>
    /// Service table
    /// </summary>
    public class ServiceTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTable"/> class.
        /// </summary>
        /// <param name="descriptors">The descriptors.</param>
        /// <param name="provider">The provider.</param>
        public ServiceTable(IEnumerable<ServiceDescriptor> descriptors, IServiceProvider provider)
        {
            Provider = provider;
            Services = new ConcurrentDictionary<Type, List<IService>>();
            GenericServices = new ConcurrentDictionary<Type, List<IGenericService>>();

            SetupDescriptors(descriptors);
            Add(typeof(IEnumerable<>), new OpenIEnumerableService(typeof(IEnumerable<>), this, ServiceLifetime.Transient));
        }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public IServiceProvider Provider { get; set; }

        /// <summary>
        /// Gets or sets the generic services.
        /// </summary>
        /// <value>The generic services.</value>
        private ConcurrentDictionary<Type, List<IGenericService>> GenericServices { get; set; }

        /// <summary>
        /// Gets or sets the services.
        /// </summary>
        /// <value>The services.</value>
        private ConcurrentDictionary<Type, List<IService>> Services { get; set; }

        /// <summary>
        /// Adds the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instanceService">The instance service.</param>
        public void Add(Type serviceType, IService instanceService)
        {
            Services.AddOrUpdate(serviceType,
                x =>
                {
                    var TempList = new List<IService>();
                    TempList.Add(instanceService);
                    return TempList;
                },
                (x, y) =>
                {
                    y.Add(instanceService);
                    return y;
                });
        }

        /// <summary>
        /// Adds the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="genericService">The generic service.</param>
        public void Add(Type serviceType, IGenericService genericService)
        {
            GenericServices.AddOrUpdate(serviceType,
                x =>
                {
                    var TempList = new List<IGenericService>();
                    TempList.Add(genericService);
                    return TempList;
                },
                (x, y) =>
                {
                    y.Add(genericService);
                    return y;
                });
        }

        public ServiceTable Copy()
        {
            foreach (var Key in Services)
            {
            }
        }

        /// <summary>
        /// Gets the service specified.
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <returns>The service associated with the parameter type.</returns>
        public IService GetService(Type parameterType)
        {
            return GetServices(parameterType).Last();
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>The list of services that satisfy the request</returns>
        public List<IService> GetServices(Type serviceType)
        {
            List<IService> ReturnValue = new List<IService>();
            if (Services.TryGetValue(serviceType, out ReturnValue))
            {
                return ReturnValue;
            }
            var ServiceTypeInfo = serviceType.GetTypeInfo();
            if (ServiceTypeInfo.IsGenericType)
            {
                Type OpenServiceType = serviceType.GetGenericTypeDefinition();
                List<IGenericService> GenericServicesReturned = new List<IGenericService>();
                if (GenericServices.TryGetValue(OpenServiceType, out GenericServicesReturned))
                {
                    foreach (var Item in GenericServicesReturned)
                    {
                        var ClosedService = Item.CreateService(serviceType);
                        if (ClosedService != null)
                        {
                            Add(serviceType, ClosedService);
                        }
                    }
                    Services.TryGetValue(serviceType, out ReturnValue);
                }
            }
            else if (!ServiceTypeInfo.IsGenericTypeDefinition &&
                        !ServiceTypeInfo.IsAbstract &&
                        !ServiceTypeInfo.IsInterface)
            {
                var TempService = new ConstructorService(serviceType, serviceType, this, ServiceLifetime.Transient);
                Add(serviceType, TempService);
                ReturnValue = new List<IService>();
                ReturnValue.Add(TempService);
            }
            return ReturnValue ?? new List<IService>();
        }

        /// <summary>
        /// Resolves the specified parameter type.
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <returns>The object of the type specified.</returns>
        public object Resolve(Type parameterType)
        {
            var ParameterTypeInfo = parameterType.GetTypeInfo();
            return GetService(parameterType).Create(Provider);
        }

        private void SetupDescriptors(IEnumerable<ServiceDescriptor> descriptors)
        {
            foreach (var Descriptor in descriptors)
            {
                var ServiceTypeInfo = Descriptor.ServiceType.GetTypeInfo();
                if (ServiceTypeInfo.IsGenericTypeDefinition)
                {
                    var implementationTypeInfo = Descriptor.ImplementationType?.GetTypeInfo();

                    if (implementationTypeInfo == null ||
                        !implementationTypeInfo.IsGenericTypeDefinition)
                    {
                        throw new ArgumentException("Implementation must be open generic");
                    }

                    if (implementationTypeInfo.IsAbstract ||
                        implementationTypeInfo.IsInterface)
                    {
                        throw new ArgumentException("Implementation must be a class that can be activated");
                    }

                    Add(Descriptor.ServiceType, new GenericService(Descriptor.ImplementationType, this, Descriptor.Lifetime));
                }
                else if (Descriptor.ImplementationInstance != null)
                {
                    Add(Descriptor.ServiceType, new InstanceService(Descriptor.ServiceType, Descriptor.ImplementationInstance, this, Descriptor.Lifetime));
                }
                else if (Descriptor.ImplementationFactory != null)
                {
                    Add(Descriptor.ServiceType, new FactoryService(Descriptor.ServiceType, Descriptor.ImplementationFactory, this, Descriptor.Lifetime));
                }
                else
                {
                    var ImplementationTypeInfo = Descriptor.ImplementationType.GetTypeInfo();

                    if (ImplementationTypeInfo.IsGenericTypeDefinition ||
                        ImplementationTypeInfo.IsAbstract ||
                        ImplementationTypeInfo.IsInterface)
                    {
                        throw new ArgumentException("Implementation must be concrete");
                    }

                    Add(Descriptor.ServiceType, new ConstructorService(Descriptor.ServiceType, Descriptor.ImplementationType, this, Descriptor.Lifetime));
                }
            }
        }
    }
}