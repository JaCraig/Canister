using Canister.Interfaces;
using Canister.IoC.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Canister.Tests.Utils
{
    public class CanisterConfigurationTests
    {
        [Fact]
        public void AddAssembly_AddsAssemblies()
        {
            var config = new CanisterConfiguration();
            var assembly = typeof(CanisterConfigurationTests).Assembly;

            config.AddAssembly(assembly);

            Assert.Contains(assembly, config.Assemblies);
        }

        [Fact]
        public void AddAssembly_Enumerable_AddsAssemblies()
        {
            var config = new CanisterConfiguration();
            var assemblies = new[] { typeof(CanisterConfigurationTests).Assembly };

            config.AddAssembly(assemblies);

            Assert.Contains(assemblies[0], config.Assemblies);
        }

        [Fact]
        public void AddAssembly_Enumerable_IgnoresNull()
        {
            var config = new CanisterConfiguration();
            config.AddAssembly((IEnumerable<Assembly>)null);

            Assert.Empty(config.Assemblies);
        }

        [Fact]
        public void AddAssembly_IgnoresNullAssemblies()
        {
            var config = new CanisterConfiguration();
            config.AddAssembly(null);

            Assert.Empty(config.Assemblies);
        }

        [Fact]
        public void AddDefaultAssemblies_AddsEntryAndExecutingAssemblies()
        {
            var config = new CanisterConfiguration();
            config.AddDefaultAssemblies();

            Assert.Contains(Assembly.GetEntryAssembly(), config.Assemblies);
            Assert.Contains(Assembly.GetExecutingAssembly(), config.Assemblies);
        }

        [Fact]
        public void AvailableInterfaces_ReturnsOnlyInterfaces()
        {
            var config = new CanisterConfiguration();
            config.AddAssembly(typeof(CanisterConfigurationTests).Assembly);

            var interfaces = config.AvailableInterfaces;

            Assert.All(interfaces, t => Assert.True(t.IsInterface));
        }

        [Fact]
        public void AvailableTypes_ReturnsOnlyConcreteClasses()
        {
            var config = new CanisterConfiguration();
            config.AddAssembly(typeof(CanisterConfigurationTests).Assembly);

            var types = config.AvailableTypes;

            Assert.All(types, t => Assert.True(t.IsClass && !t.IsAbstract));
        }

        [Fact]
        public void Log_DoesNotThrowIfLoggerIsNull()
        {
            var config = new CanisterConfiguration();
            config.UseLogger(null);

            var result = config.Log(LogLevel.Information, "Test");
            Assert.NotNull(result);
        }

        [Fact]
        public void Types_ReturnsAllTypes()
        {
            var config = new CanisterConfiguration();
            config.AddAssembly(typeof(CanisterConfigurationTests).Assembly);

            var types = config.Types;

            Assert.Contains(typeof(CanisterConfigurationTests), types);
        }

        [Fact]
        public void UseLogger_SetsLoggerAndLogLevel()
        {
            var config = new CanisterConfiguration();
            var logger = new TestLogger();

            config.UseLogger(logger, LogLevel.Debug);

            config.Log(LogLevel.Debug, "Test message");
            Assert.True(logger.WasLogged);
        }

        private class TestLogger : ILogger
        {
            public bool WasLogged { get; private set; }

            public IDisposable BeginScope<TState>(TState state) => NullLogger.Instance.BeginScope(state);

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                WasLogged = true;
            }
        }
    }
}