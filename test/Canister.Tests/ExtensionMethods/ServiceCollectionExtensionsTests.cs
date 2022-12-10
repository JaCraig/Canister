using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Canister.Tests.ExtensionMethods
{
    public class ModuleTestClass
    { }

    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddCanisterModulesDefaultAssemblyDiscover()
        {
            ServiceProvider? Provider = GetServices().AddCanisterModules()?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddCanisterModulesSpecifyAssemblies()
        {
            ServiceProvider? Provider = GetServices().AddCanisterModules(x => x.AddAssembly(typeof(ServiceCollectionExtensionsTests).Assembly))?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());
        }

        private static IServiceCollection GetServices() => new ServiceCollection();
    }

    public class TestModule : IModule
    {
        public int Order => 0;

        public void Load(IServiceCollection serviceDescriptors) => serviceDescriptors.AddTransient(_ => new ModuleTestClass());
    }
}