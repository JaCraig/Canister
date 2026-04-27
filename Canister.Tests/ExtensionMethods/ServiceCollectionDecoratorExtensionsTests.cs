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
        public void Decorate_WorksWithSingletonFactory()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ITestService>(_ => new TestService());
            services.Decorate<ITestService, TestServiceDecorator>();

            var provider = services.BuildServiceProvider();
            var service = provider.GetRequiredService<ITestService>();

            Assert.IsType<TestServiceDecorator>(service);
            Assert.Equal("decorated(original)", service.GetValue());
        }

        [Fact]
        public void Decorate_WorksWithKeyedSingletonInstance()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<ITestService>("B", new TestService());
            services.Decorate<ITestService, TestServiceDecorator>();

            var provider = services.BuildServiceProvider();
            var service = provider.GetKeyedService<ITestService>("B");

            Assert.NotNull(service);
            Assert.IsType<TestServiceDecorator>(service);
            Assert.Equal("decorated(original)", service.GetValue());
        }

        [Fact]
        public void Decorate_WorksWithKeyedScopedType()
        {
            var services = new ServiceCollection();
            services.AddKeyedScoped<ITestService, TestService>("S");
            services.Decorate<ITestService, TestServiceDecorator>();

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var service = scope.ServiceProvider.GetKeyedService<ITestService>("S");

            Assert.NotNull(service);
            Assert.IsType<TestServiceDecorator>(service);
            Assert.Equal("decorated(original)", service.GetValue());
        }

        [Fact]
        public void Decorate_WorksWithKeyedFactory()
        {
            var services = new ServiceCollection();
            services.AddKeyedTransient<ITestService>("A", (_, __) => new TestService());
            services.Decorate<ITestService, TestServiceDecorator>();
            var provider = services.BuildServiceProvider();
            var service = provider.GetKeyedService<ITestService>("A");
            Assert.NotNull(service);
            Assert.IsType<TestServiceDecorator>(service);
            Assert.Equal("decorated(original)", service.GetValue());
            Assert.Null(provider.GetService<ITestService>());
        }

        [Fact]
        public void Decorate_WorksWithKeyedType()
        {
            var services = new ServiceCollection();
            services.AddKeyedTransient<ITestService, TestService>("A");
            services.Decorate<ITestService, TestServiceDecorator>();
            var provider = services.BuildServiceProvider();
            var service = provider.GetKeyedService<ITestService>("A");
            Assert.NotNull(service);
            Assert.IsType<TestServiceDecorator>(service);
            Assert.Equal("decorated(original)", service.GetValue());
            Assert.Null(provider.GetService<ITestService>());
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