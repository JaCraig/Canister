using Canister.Default;
using Canister.Default.Services;
using Canister.Tests.Default.Services.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;

namespace Canister.Tests.Default.Services.BaseClasses
{
    public class ServiceTestBase
    {
        public ServiceTestBase()
        {
            ServiceProvider = new DefaultBootstrapper(new List<Assembly>(), new List<ServiceDescriptor>());
            Table = ServiceProvider.AppContainer;
            Table.Add(typeof(ISimpleInterface), "", new InstanceService(typeof(SimpleClassWithAbstractParent), new SimpleClassWithAbstractParent(), Table, ServiceLifetime.Transient));
            Table.Add(typeof(ISimpleInterface), "", new InstanceService(typeof(SimpleClassWithDefaultConstructor), new SimpleClassWithDefaultConstructor(), Table, ServiceLifetime.Transient));
            Table.Add(typeof(SimpleAbstractClass), "", new InstanceService(typeof(SimpleClassWithAbstractParent), new SimpleClassWithAbstractParent(), Table, ServiceLifetime.Transient));
            Table.Add(typeof(IGenericInterface<>), "", new GenericService(typeof(GenericClass<>), Table, ServiceLifetime.Transient));
            Table.Add(typeof(int), "", new InstanceService(typeof(int), 1, Table, ServiceLifetime.Transient));
            Table.Add(typeof(int), "", new InstanceService(typeof(int), 2, Table, ServiceLifetime.Transient));
            Table.Add(typeof(string), "", new InstanceService(typeof(string), "", Table, ServiceLifetime.Transient));
        }

        public DefaultBootstrapper ServiceProvider { get; set; }

        public ServiceTable Table { get; set; }
    }
}