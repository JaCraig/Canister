using Canister.Default;
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
            ServiceProvider = new DefaultBootstrapper(new List<Assembly>(), new ServiceCollection());
            Table = ServiceProvider.AppContainer;
            Table.AddTransient(typeof(ISimpleInterface), x => new SimpleClassWithAbstractParent());
            Table.AddTransient(typeof(ISimpleInterface), x => new SimpleClassWithDefaultConstructor());
            Table.AddTransient(typeof(SimpleAbstractClass), x => new SimpleClassWithAbstractParent());
            Table.AddTransient(typeof(IGenericInterface<>), typeof(GenericClass<>));
            Table.AddTransient(typeof(int), x => 1);
            Table.AddTransient(typeof(int), x => 2);
            Table.AddTransient(typeof(string), x => "");
        }

        public DefaultBootstrapper ServiceProvider { get; set; }

        public IServiceCollection Table { get; set; }
    }
}