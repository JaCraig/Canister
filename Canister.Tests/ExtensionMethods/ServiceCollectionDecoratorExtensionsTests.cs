using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Canister.Tests.ExtensionMethods
{
    public class ServiceCollectionDecoratorExtensionsTests
    {
        [Fact]
        public void Decorate_ReturnsIfNoRegistration()
        {
            var services = new ServiceCollection();
            services.Decorate<ITestService, TestServiceDecorator>();
            var provider = services.BuildServiceProvider();
            var service = provider.GetService<ITestService>();
            Assert.Null(service);
        }

        [Fact]
        public void Decorate_WorksWithSingletonInstance()
        {
            var instance = new TestService();
            var services = new ServiceCollection();
            services.AddSingleton<ITestService>(instance);
            services.Decorate<ITestService, TestServiceDecorator>();
            var provider = services.BuildServiceProvider();
            var service = provider.GetRequiredService<ITestService>();
            Assert.IsType<TestServiceDecorator>(service);
            Assert.Equal("decorated(original)", service.GetValue());
        }

        [Fact]
        public void Decorate_WrapsServiceWithDecorator()
        {
            var services = new ServiceCollection();
            services.AddTransient<ITestService, TestService>();
            services.Decorate<ITestService, TestServiceDecorator>();
            var provider = services.BuildServiceProvider();
            var service = provider.GetRequiredService<ITestService>();
            Assert.IsType<TestServiceDecorator>(service);
            Assert.Equal("decorated(original)", service.GetValue());
        }

        public interface ITestService
        {
            string GetValue();
        }

        public class TestService : ITestService
        {
            public string GetValue() => "original";
        }

        public class TestServiceDecorator : ITestService
        {
            public TestServiceDecorator(ITestService inner) => _inner = inner;

            private readonly ITestService _inner;

            public string GetValue() => $"decorated({_inner.GetValue()})";
        }
    }
}