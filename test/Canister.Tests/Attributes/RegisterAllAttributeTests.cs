using Canister.IoC.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace Canister.Tests.Attributes
{
    [RegisterAll(ServiceLifetime.Scoped)]
    public interface IScopedRegisterAllTestInterface
    {
    }

    [RegisterAll(ServiceLifetime.Singleton)]
    public interface ISingletonRegisterAllTestInterface
    {
    }

    [RegisterAll(ServiceLifetime.Transient)]
    public interface ITransientRegisterAllTestInterface
    {
    }

    public class RegisterAllAttributeTests
    {
        [Fact]
        public void RegisterAllAttributeTests_Register()
        {
            var Container = new ServiceCollection();
            _ = Container.AddCanisterModules();
            ServiceProvider ServiceProvider = Container.BuildServiceProvider();
            IScopedRegisterAllTestInterface? Instance = ServiceProvider.GetService<IScopedRegisterAllTestInterface>();
            Assert.NotNull(Instance);
        }

        [Fact]
        public void RegisterAllAttributeTests_RegisterAll()
        {
            var Container = new ServiceCollection();
            _ = Container.AddCanisterModules();
            ServiceProvider ServiceProvider = Container.BuildServiceProvider();
            System.Collections.Generic.IEnumerable<IScopedRegisterAllTestInterface> ScopedServices = ServiceProvider.GetServices<IScopedRegisterAllTestInterface>();
            Assert.NotNull(ScopedServices);
            Assert.NotEmpty(ScopedServices);
            Assert.Equal(3, ScopedServices.Count());
        }

        [Fact]
        public void RegisterAllAttributeTests_RegisterAllSingleton()
        {
            var Container = new ServiceCollection();
            _ = Container.AddCanisterModules();
            ServiceProvider ServiceProvider = Container.BuildServiceProvider();
            System.Collections.Generic.IEnumerable<ISingletonRegisterAllTestInterface> SingletonServices = ServiceProvider.GetServices<ISingletonRegisterAllTestInterface>();
            Assert.NotNull(SingletonServices);
            Assert.NotEmpty(SingletonServices);
            Assert.Equal(3, SingletonServices.Count());
        }

        [Fact]
        public void RegisterAllAttributeTests_RegisterAllTransient()
        {
            var Container = new ServiceCollection();
            _ = Container.AddCanisterModules();
            ServiceProvider ServiceProvider = Container.BuildServiceProvider();
            System.Collections.Generic.IEnumerable<ITransientRegisterAllTestInterface> TransientServices = ServiceProvider.GetServices<ITransientRegisterAllTestInterface>();
            Assert.NotNull(TransientServices);
            Assert.NotEmpty(TransientServices);
            Assert.Equal(2, TransientServices.Count());
        }

        [Fact]
        public void RegisterAllAttributeTests_RegisterSingleton()
        {
            var Container = new ServiceCollection();
            _ = Container.AddCanisterModules();
            ServiceProvider ServiceProvider = Container.BuildServiceProvider();
            ISingletonRegisterAllTestInterface? Instance = ServiceProvider.GetService<ISingletonRegisterAllTestInterface>();
            Assert.NotNull(Instance);
        }

        [Fact]
        public void RegisterAllAttributeTests_RegisterTransient()
        {
            var Container = new ServiceCollection();
            _ = Container.AddCanisterModules();
            ServiceProvider ServiceProvider = Container.BuildServiceProvider();
            ITransientRegisterAllTestInterface? Instance = ServiceProvider.GetService<ITransientRegisterAllTestInterface>();
            Assert.NotNull(Instance);
        }
    }

    public class ScopedTestClass1 : IScopedRegisterAllTestInterface
    {
    }

    public class ScopedTestClass2 : IScopedRegisterAllTestInterface
    {
    }

    public class ScopedTestClass3 : IScopedRegisterAllTestInterface
    {
    }

    public class SingletonTestClass1 : ISingletonRegisterAllTestInterface
    {
    }

    public class SingletonTestClass2 : ISingletonRegisterAllTestInterface
    {
    }

    public class SingletonTestClass3 : ISingletonRegisterAllTestInterface
    {
    }

    public class TransientTestClass1 : ITransientRegisterAllTestInterface
    {
    }

    public class TransientTestClass2 : ITransientRegisterAllTestInterface
    {
    }
}