using Canister.IoC.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Canister.Tests.ExtensionMethods
{
    [RegisterAll(ServiceLifetime.Singleton)]
    public interface IInterfaceToAdd;

    public interface IInterfaceToAdd2;

    [Register(ServiceLifetime.Singleton)]
    public class ClassToAdd : IInterfaceToAdd2;

    public class ClassToAdd2 : IInterfaceToAdd;

    public class ClassToAdd3 : IInterfaceToAdd;

    public class ClassToNotAdd;

    public class ServiceCollectionAddByAttribute
    {
        [Fact]
        public void ServiceCollectionAddByAttributeTest()
        {
            var services = new ServiceCollection();
            _ = services.AddCanisterModules();
            ServiceProvider provider = services.BuildServiceProvider();

            IInterfaceToAdd2? instance = provider.GetService<IInterfaceToAdd2>();
            Assert.NotNull(instance);

            IEnumerable<IInterfaceToAdd> Instances = provider.GetServices<IInterfaceToAdd>();
            Assert.NotNull(Instances);
            Assert.Equal(2, Instances.Count());

            ClassToAdd? instance3 = provider.GetService<ClassToAdd>();
            Assert.NotNull(instance3);

            ClassToNotAdd? instance6 = provider.GetService<ClassToNotAdd>();
            Assert.Null(instance6);
        }
    }
}