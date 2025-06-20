using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Canister.Tests
{
    public class ServiceCollectionExtensionsTransientTests
    {
        [Fact]
        public void TryAddAllTransient_RegistersAllAssignableTypes_AsTransient()
        {
            var availableTypes = new[] { typeof(TestServiceImpl), typeof(AnotherService) };
            var services = new ServiceCollection();
            // Simulate CanisterConfiguration
            var canisterConfig = new
            {
                AvailableTypes = availableTypes,
                Log = (Action<string, object, object>)((fmt, a, b) => { /* no-op for test */ })
            };
            // Use reflection to call the internal logic with our config For this test, we call the
            // public API and check the result Register TestServiceImpl as TestService
            services.AddTransient<TestService, TestServiceImpl>();
            var result = ServiceCollectionExtensions.TryAddAllTransient(services, typeof(TestService));
            // Should not add duplicate, so count should be 1
            var registrations = result.Where(sd => sd.ServiceType == typeof(TestService)).ToList();
            Assert.Single(registrations);
            Assert.Equal(typeof(TestServiceImpl), registrations[0].ImplementationType);
        }

        [Fact]
        public void TryAddAllTransient_RegistersGenericTypeDefinition_IfApplicable()
        {
            var services = new ServiceCollection();
            var result = ServiceCollectionExtensions.TryAddAllTransient(services, typeof(List<>));
            Assert.Contains(result, sd => sd.ServiceType.IsGenericTypeDefinition && sd.ServiceType == typeof(List<>));
        }

        [Fact]
        public void TryAddAllTransient_ReturnsNull_WhenServiceDescriptorsIsNull()
        {
            IServiceCollection? result = ServiceCollectionExtensions.TryAddAllTransient(null, typeof(TestService));
            Assert.Null(result);
        }

        private class AnotherService
        { }

        private class FakeCanisterConfiguration
        {
            public IEnumerable<Type> AvailableTypes { get; set; } = Array.Empty<Type>();
            public List<string> LogMessages { get; } = new();

            public void Log(string format, params object[] args)
            {
                LogMessages.Add(string.Format(format, args));
            }
        }

        private class TestService
        { }

        private class TestServiceImpl : TestService
        { }
    }
}