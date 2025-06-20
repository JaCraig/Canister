using Canister.IoC.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Canister.Tests.Attributes
{
    public interface ITestInterface;

    public class RegisterAttributeTests
    {
        [Fact]
        public void RegisterAttributeTests_Register()
        {
            var Container = new ServiceCollection();
            _ = Container.AddCanisterModules();
            ServiceProvider ServiceProvider = Container.BuildServiceProvider();
            TestClass? Instance = ServiceProvider.GetService<TestClass>();
            Assert.NotNull(Instance);
        }

        [Fact]
        public void RegisterAttributeTests_RegisterWithKey()
        {
            var Container = new ServiceCollection();
            _ = Container.AddCanisterModules();
            ServiceProvider ServiceProvider = Container.BuildServiceProvider();
            TestClassWithKey? Instance = ServiceProvider.GetKeyedService<TestClassWithKey>("test");
            Assert.NotNull(Instance);
        }

        [Fact]
        public void RegisterAttributeTests_RegisterWithKeyAndType()
        {
            var Container = new ServiceCollection();
            _ = Container.AddCanisterModules();
            ServiceProvider ServiceProvider = Container.BuildServiceProvider();
            ITestInterface? Instance = ServiceProvider.GetKeyedService<ITestInterface>("test");
            Assert.NotNull(Instance);
        }
    }

    [Register(ServiceLifetime.Singleton)]
    public class TestClass : ITestInterface;

    [Register(ServiceLifetime.Singleton, "test")]
    public class TestClassWithKey : ITestInterface;
}