using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Canister.Tests.ExtensionMethods
{
    public interface IModuleTestInterface;

    public class ModuleTestClass;

    public class ModuleTestClass1 : IModuleTestInterface;

    public class ModuleTestClass2 : IModuleTestInterface;

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

        [Fact]
        public void AddKeyedScopedIfTest()
        {
            ServiceProvider? Provider = GetServices().AddKeyedScopedIf<ModuleTestClass>(_ => true, "A")?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedScopedIf<ModuleTestClass>(_ => false, "A")?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedScopedIfTest1()
        {
            ServiceProvider? Provider = GetServices().AddKeyedScopedIf(_ => true, "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedScopedIf(_ => false, "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedScopedIfTest2()
        {
            ServiceProvider? Provider = GetServices().AddKeyedScopedIf<ModuleTestClass, ModuleTestClass>(_ => true, "A")?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedScopedIf<ModuleTestClass, ModuleTestClass>(_ => false, "A")?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedScopedIfTest3()
        {
            ServiceProvider? Provider = GetServices().AddKeyedScopedIf<ModuleTestClass, ModuleTestClass>(_ => true, "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedScopedIf<ModuleTestClass, ModuleTestClass>(_ => false, "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedScopedIfTest4()
        {
            ServiceProvider? Provider = GetServices().AddKeyedScopedIf(_ => true, typeof(ModuleTestClass), "A")?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedScopedIf(_ => false, typeof(ModuleTestClass), "A")?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedScopedIfTest5()
        {
            ServiceProvider? Provider = GetServices().AddKeyedScopedIf(_ => true, typeof(ModuleTestClass), "A", typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedScopedIf(_ => false, typeof(ModuleTestClass), "A", typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedScopedIfTest6()
        {
            ServiceProvider? Provider = GetServices().AddKeyedScopedIf(_ => true, typeof(ModuleTestClass), "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedScopedIf(_ => false, typeof(ModuleTestClass), "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedSingletonIfTest()
        {
            ServiceProvider? Provider = GetServices().AddKeyedSingletonIf<ModuleTestClass>(_ => true, "A")?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedSingletonIf<ModuleTestClass>(_ => false, "A")?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedSingletonIfTest1()
        {
            ServiceProvider? Provider = GetServices().AddKeyedSingletonIf(_ => true, "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedSingletonIf(_ => false, "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedSingletonIfTest2()
        {
            ServiceProvider? Provider = GetServices().AddKeyedSingletonIf<ModuleTestClass, ModuleTestClass>(_ => true, "A")?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedSingletonIf<ModuleTestClass, ModuleTestClass>(_ => false, "A")?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedSingletonIfTest3()
        {
            ServiceProvider? Provider = GetServices().AddKeyedSingletonIf<ModuleTestClass, ModuleTestClass>(_ => true, "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedSingletonIf<ModuleTestClass, ModuleTestClass>(_ => false, "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedSingletonIfTest4()
        {
            ServiceProvider? Provider = GetServices().AddKeyedSingletonIf(_ => true, typeof(ModuleTestClass), "A")?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedSingletonIf(_ => false, typeof(ModuleTestClass), "A")?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedSingletonIfTest5()
        {
            ServiceProvider? Provider = GetServices().AddKeyedSingletonIf(_ => true, typeof(ModuleTestClass), "A", typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedSingletonIf(_ => false, typeof(ModuleTestClass), "A", typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedSingletonIfTest6()
        {
            ServiceProvider? Provider = GetServices().AddKeyedSingletonIf(_ => true, typeof(ModuleTestClass), "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedSingletonIf(_ => false, typeof(ModuleTestClass), "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedTransientIfTest()
        {
            ServiceProvider? Provider = GetServices().AddKeyedTransientIf<ModuleTestClass>(_ => true, "A")?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedTransientIf<ModuleTestClass>(_ => false, "A")?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedTransientIfTest1()
        {
            ServiceProvider? Provider = GetServices().AddKeyedTransientIf(_ => true, "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedTransientIf(_ => false, "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedTransientIfTest2()
        {
            ServiceProvider? Provider = GetServices().AddKeyedTransientIf<ModuleTestClass, ModuleTestClass>(_ => true, "A")?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedTransientIf<ModuleTestClass, ModuleTestClass>(_ => false, "A")?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedTransientIfTest3()
        {
            ServiceProvider? Provider = GetServices().AddKeyedTransientIf<ModuleTestClass, ModuleTestClass>(_ => true, "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedTransientIf<ModuleTestClass, ModuleTestClass>(_ => false, "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedTransientIfTest4()
        {
            ServiceProvider? Provider = GetServices().AddKeyedTransientIf(_ => true, typeof(ModuleTestClass), "A")?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedTransientIf(_ => false, typeof(ModuleTestClass), "A")?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedTransientIfTest5()
        {
            ServiceProvider? Provider = GetServices().AddKeyedTransientIf(_ => true, typeof(ModuleTestClass), "A", typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedTransientIf(_ => false, typeof(ModuleTestClass), "A", typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddKeyedTransientIfTest6()
        {
            ServiceProvider? Provider = GetServices().AddKeyedTransientIf(_ => true, typeof(ModuleTestClass), "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetKeyedService<ModuleTestClass>("A"));

            Provider = GetServices().AddKeyedTransientIf(_ => false, typeof(ModuleTestClass), "A", (_, __) => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetKeyedService<ModuleTestClass>("A"));
        }

        [Fact]
        public void AddScopedIfTest()
        {
            ServiceProvider? Provider = GetServices().AddScopedIf<ModuleTestClass>(_ => true)?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddScopedIf<ModuleTestClass>(_ => false)?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddScopedIfTest1()
        {
            ServiceProvider? Provider = GetServices().AddScopedIf(_ => true, _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddScopedIf(_ => false, _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddScopedIfTest2()
        {
            ServiceProvider? Provider = GetServices().AddScopedIf<ModuleTestClass, ModuleTestClass>(_ => true)?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddScopedIf<ModuleTestClass, ModuleTestClass>(_ => false)?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddScopedIfTest3()
        {
            ServiceProvider? Provider = GetServices().AddScopedIf<ModuleTestClass, ModuleTestClass>(_ => true, _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddScopedIf<ModuleTestClass, ModuleTestClass>(_ => false, _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddScopedIfTest4()
        {
            ServiceProvider? Provider = GetServices().AddScopedIf(_ => true, typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddScopedIf(_ => false, typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddScopedIfTest5()
        {
            ServiceProvider? Provider = GetServices().AddScopedIf(_ => true, typeof(ModuleTestClass), typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddScopedIf(_ => false, typeof(ModuleTestClass), typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddScopedIfTest6()
        {
            ServiceProvider? Provider = GetServices().AddScopedIf(_ => true, typeof(ModuleTestClass), _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddScopedIf(_ => false, typeof(ModuleTestClass), _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddSingletonIfTest()
        {
            ServiceProvider? Provider = GetServices().AddSingletonIf<ModuleTestClass>(_ => true)?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddSingletonIf<ModuleTestClass>(_ => false)?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddSingletonIfTest1()
        {
            ServiceProvider? Provider = GetServices().AddSingletonIf(_ => true, _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddSingletonIf(_ => false, _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddSingletonIfTest2()
        {
            ServiceProvider? Provider = GetServices().AddSingletonIf<ModuleTestClass, ModuleTestClass>(_ => true)?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddSingletonIf<ModuleTestClass, ModuleTestClass>(_ => false)?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddSingletonIfTest3()
        {
            ServiceProvider? Provider = GetServices().AddSingletonIf<ModuleTestClass, ModuleTestClass>(_ => true, _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddSingletonIf<ModuleTestClass, ModuleTestClass>(_ => false, _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddSingletonIfTest4()
        {
            ServiceProvider? Provider = GetServices().AddSingletonIf(_ => true, typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddSingletonIf(_ => false, typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddSingletonIfTest5()
        {
            ServiceProvider? Provider = GetServices().AddSingletonIf(_ => true, typeof(ModuleTestClass), typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddSingletonIf(_ => false, typeof(ModuleTestClass), typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddSingletonIfTest6()
        {
            ServiceProvider? Provider = GetServices().AddSingletonIf(_ => true, typeof(ModuleTestClass), _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddSingletonIf(_ => false, typeof(ModuleTestClass), _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddTransientIfTest()
        {
            ServiceProvider? Provider = GetServices().AddTransientIf<ModuleTestClass>(_ => true)?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddTransientIf<ModuleTestClass>(_ => false)?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddTransientIfTest1()
        {
            ServiceProvider? Provider = GetServices().AddTransientIf(_ => true, _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddTransientIf(_ => false, _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddTransientIfTest2()
        {
            ServiceProvider? Provider = GetServices().AddTransientIf<ModuleTestClass, ModuleTestClass>(_ => true)?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddTransientIf<ModuleTestClass, ModuleTestClass>(_ => false)?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddTransientIfTest3()
        {
            ServiceProvider? Provider = GetServices().AddTransientIf<ModuleTestClass, ModuleTestClass>(_ => true, _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddTransientIf<ModuleTestClass, ModuleTestClass>(_ => false, _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddTransientIfTest4()
        {
            ServiceProvider? Provider = GetServices().AddTransientIf(_ => true, typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddTransientIf(_ => false, typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddTransientIfTest5()
        {
            ServiceProvider? Provider = GetServices().AddTransientIf(_ => true, typeof(ModuleTestClass), typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddTransientIf(_ => false, typeof(ModuleTestClass), typeof(ModuleTestClass))?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void AddTransientIfTest6()
        {
            ServiceProvider? Provider = GetServices().AddTransientIf(_ => true, typeof(ModuleTestClass), _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ModuleTestClass>());

            Provider = GetServices().AddTransientIf(_ => false, typeof(ModuleTestClass), _ => new ModuleTestClass())?.BuildServiceProvider();
            Assert.Null(Provider?.GetService<ModuleTestClass>());
        }

        [Fact]
        public void Exists()
        {
            Assert.True(GetServices().AddTransient<ModuleTestClass1>().Exists<ModuleTestClass1>());
            Assert.False(GetServices().AddTransient<ModuleTestClass1>().Exists<ModuleTestClass2>());
            Assert.True(GetServices().AddTransient<IModuleTestInterface, ModuleTestClass1>().Exists<IModuleTestInterface>());
            Assert.False(GetServices().AddTransient<IModuleTestInterface, ModuleTestClass1>().Exists<ModuleTestClass2>());
            Assert.True(GetServices().AddTransient<IModuleTestInterface, ModuleTestClass1>().Exists(typeof(IModuleTestInterface)));
            Assert.False(GetServices().AddTransient<IModuleTestInterface, ModuleTestClass1>().Exists(typeof(ModuleTestClass2)));
            Assert.True(GetServices().AddTransient<IModuleTestInterface, ModuleTestClass1>().Exists<IModuleTestInterface, ModuleTestClass1>());
            Assert.False(GetServices().AddTransient<IModuleTestInterface, ModuleTestClass1>().Exists<IModuleTestInterface, ModuleTestClass2>());
            Assert.True(GetServices().AddTransient<IModuleTestInterface, ModuleTestClass1>().Exists(typeof(IModuleTestInterface), typeof(ModuleTestClass1)));
            Assert.False(GetServices().AddTransient<IModuleTestInterface, ModuleTestClass1>().Exists(typeof(IModuleTestInterface), typeof(ModuleTestClass2)));
        }

        [Fact]
        public void ExistsKeyed()
        {
            Assert.True(GetServices().AddKeyedTransient<ModuleTestClass1>("A").Exists<ModuleTestClass1>("A"));
            Assert.False(GetServices().AddKeyedTransient<ModuleTestClass1>("A").Exists<ModuleTestClass2>("A"));
            Assert.True(GetServices().AddKeyedTransient<IModuleTestInterface, ModuleTestClass1>("A").Exists<IModuleTestInterface>("A"));
            Assert.False(GetServices().AddKeyedTransient<IModuleTestInterface, ModuleTestClass1>("A").Exists<ModuleTestClass2>("A"));
            Assert.True(GetServices().AddKeyedTransient<IModuleTestInterface, ModuleTestClass1>("A").Exists(typeof(IModuleTestInterface), "A"));
            Assert.False(GetServices().AddKeyedTransient<IModuleTestInterface, ModuleTestClass1>("A").Exists(typeof(ModuleTestClass2), "A"));
            Assert.True(GetServices().AddKeyedTransient<IModuleTestInterface, ModuleTestClass1>("A").Exists(typeof(IModuleTestInterface), "A"));
            Assert.False(GetServices().AddKeyedTransient<IModuleTestInterface, ModuleTestClass1>("A").Exists(typeof(ModuleTestClass1), "A"));
        }

        private static IServiceCollection GetServices() => new ServiceCollection();
    }

    public class TestModule : IModule
    {
        public int Order => 0;

        public void Load(IServiceCollection serviceDescriptors) => serviceDescriptors.AddTransient(_ => new ModuleTestClass());
    }
}