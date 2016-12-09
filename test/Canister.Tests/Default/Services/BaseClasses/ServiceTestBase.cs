using Canister.Default.Services;
using Canister.Tests.Default.Services.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Canister.Tests.Default.Services.BaseClasses
{
    public class ServiceTestBase
    {
        public ServiceTestBase()
        {
            Table = new ServiceTable(new List<ServiceDescriptor>(), null);
            Table.Add(typeof(ISimpleInterface), "", new InstanceService(typeof(SimpleClassWithAbstractParent), new SimpleClassWithAbstractParent(), Table, ServiceLifetime.Transient));
            Table.Add(typeof(ISimpleInterface), "", new InstanceService(typeof(SimpleClassWithDefaultConstructor), new SimpleClassWithDefaultConstructor(), Table, ServiceLifetime.Transient));
            Table.Add(typeof(SimpleAbstractClass), "", new InstanceService(typeof(SimpleClassWithAbstractParent), new SimpleClassWithAbstractParent(), Table, ServiceLifetime.Transient));
            Table.Add(typeof(IGenericInterface<>), "", new GenericService(typeof(GenericClass<>), Table, ServiceLifetime.Transient));
            Table.Add(typeof(int), "", new InstanceService(typeof(int), 1, Table, ServiceLifetime.Transient));
            Table.Add(typeof(int), "", new InstanceService(typeof(int), 2, Table, ServiceLifetime.Transient));
        }

        public ServiceTable Table { get; set; }
    }
}