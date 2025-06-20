using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Xunit;

namespace Canister.Tests.ExtensionMethods
{
    public class ServiceCollectionDiagnosticsExtensionsTests
    {
        [Fact]
        public void GetRegistrationsSummary_ReturnsExpectedString()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestServiceImpl>();
            services.AddTransient<TestServiceImpl>();
            var summary = services.GetRegistrationsSummary();
            Assert.Contains("ServiceType: Canister.Tests.ExtensionMethods.ServiceCollectionDiagnosticsExtensionsTests+ITestService", summary);
            Assert.Contains("ImplementationType: Canister.Tests.ExtensionMethods.ServiceCollectionDiagnosticsExtensionsTests+TestServiceImpl", summary);
            Assert.Contains("Lifetime: Singleton", summary);
            Assert.Contains("Lifetime: Transient", summary);
        }

        [Fact]
        public void LogRegistrations_LogsToLogger()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestServiceImpl>();
            var logger = new TestLogger();
            services.LogRegistrations(logger);
            Assert.True(logger.Logged);
            Assert.Contains("ServiceType: Canister.Tests.ExtensionMethods.ServiceCollectionDiagnosticsExtensionsTests+ITestService", logger.LastMessage);
        }

        private interface ITestService { }
        private class TestServiceImpl : ITestService { }

        private class TestLogger : ILogger
        {
            public bool Logged { get; private set; }
            public string LastMessage { get; private set; } = string.Empty;
            public IDisposable BeginScope<TState>(TState state) => null!;
            public bool IsEnabled(LogLevel logLevel) => true;
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                Logged = true;
                LastMessage = formatter(state, exception);
            }
        }
    }
}