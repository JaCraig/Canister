using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Canister.Tests.Default
{
    public class ServiceCollectionExtensionsAddAll
    {
        [Fact]
        public void RegisterAllGenericScoped()
        {
            IServiceCollection Temp = GetBootstrapper();
            ServiceProvider? Provider = Temp.AddAllScoped(typeof(GenericTestClass<>))?.AddTransient(_ => "").BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<GenericTestClass<string>>());
        }

        [Fact]
        public void RegisterAllGenericSingleton()
        {
            IServiceCollection Temp = GetBootstrapper();
            ServiceProvider? Provider = Temp.AddAllSingleton(typeof(GenericTestClass<>))?.AddTransient(_ => "").BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<GenericTestClass<string>>());
        }

        [Fact]
        public void RegisterAllGenericTransient()
        {
            IServiceCollection Temp = GetBootstrapper();
            ServiceProvider? Provider = Temp.AddAllTransient(typeof(GenericTestClass<>))?.AddTransient(_ => "").BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<GenericTestClass<string>>());
        }

        [Fact]
        public void RegisterAllScoped()
        {
            IServiceCollection Temp = GetBootstrapper();
            ServiceProvider? Provider = Temp.AddAllScoped<ITestClass>()?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ITestClass>());
            Assert.Equal(2, Provider.GetServices<ITestClass>().Count());
            Assert.NotNull(Provider.GetService<TestClass>());
            Assert.NotNull(Provider.GetService<TestClass2>());
        }

        [Fact]
        public void RegisterAllSingleton()
        {
            IServiceCollection Temp = GetBootstrapper();
            ServiceProvider? Provider = Temp.AddAllSingleton<ITestClass>()?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ITestClass>());
            Assert.Equal(2, Provider.GetServices<ITestClass>().Count());
            Assert.NotNull(Provider.GetService<TestClass>());
            Assert.NotNull(Provider.GetService<TestClass2>());
        }

        [Fact]
        public void RegisterAllTransient()
        {
            IServiceCollection Temp = GetBootstrapper();
            ServiceProvider? Provider = Temp.AddAllTransient<ITestClass>()?.BuildServiceProvider();
            Assert.NotNull(Provider?.GetService<ITestClass>());
            Assert.Equal(2, Provider.GetServices<ITestClass>().Count());
            Assert.NotNull(Provider.GetService<TestClass>());
            Assert.NotNull(Provider.GetService<TestClass2>());
        }

        private static IServiceCollection GetBootstrapper() => new ServiceCollection();

        protected interface ITestClass
        {
            int A { get; set; }
        }

        protected class GenericTestClass<T>
        {
            public GenericTestClass(T param1)
            {
                Value = param1;
            }

            public T Value { get; set; }
        }

        protected class TestClass : ITestClass
        {
            public int A { get; set; }
        }

        protected class TestClass2 : ITestClass
        {
            public int A { get; set; }
        }

        protected class TestClass3
        {
            public TestClass3()
            {
            }

            public TestClass3(IEnumerable<ITestClass> classes)
            {
                Classes = classes;
            }

            public IEnumerable<ITestClass>? Classes { get; set; }
        }

        protected class TestClass4
        {
            public TestClass4()
            {
            }

            public TestClass4(TestClass3 @class)
            {
                Class = @class;
            }

            public TestClass3? Class { get; set; }
        }
    }
}