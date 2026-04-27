using Canister.Interfaces;
using Canister.IoC.Utils;
using Microsoft.Extensions.DependencyInjection;
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
            config.AddAssembly((IEnumerable<Assembly>?)null);

            Assert.Empty(config.Assemblies);
        }

        [Fact]
        public void AddAssembly_Enumerable_IgnoresEmpty()
        {
            var config = new CanisterConfiguration();
            config.AddAssembly([]);

            Assert.Empty(config.Assemblies);
        }

        [Fact]
        public void AddAssembly_Enumerable_SkipsNullAndDuplicateAssemblies()
        {
            var config = new CanisterConfiguration();
            var assembly = typeof(CanisterConfigurationTests).Assembly;

            config.AddAssembly([assembly, null, assembly]);

            Assert.Single(config.Assemblies);
            Assert.Contains(assembly, config.Assemblies);
        }

        [Fact]
        public void AddAssembly_IgnoresNullAssemblies()
        {
            var config = new CanisterConfiguration();
            config.AddAssembly(null);

            Assert.Empty(config.Assemblies);
        }

        [Fact]
        public void AddAssembly_Params_IgnoresEmptyArrays()
        {
            var config = new CanisterConfiguration();
            config.AddAssembly();

            Assert.Empty(config.Assemblies);
        }

        [Fact]
        public void AddDefaultAssemblies_AddsEntryAndExecutingAssemblies()
        {
            var config = new CanisterConfiguration();
            config.AddDefaultAssemblies();

            Assert.Contains(Assembly.GetEntryAssembly(), config.Assemblies!);
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
        public void AvailableInterfaces_CachesComputedResult()
        {
            var config = new CanisterConfiguration();
            config.AddAssembly(typeof(CanisterConfigurationTests).Assembly);

            var first = config.AvailableInterfaces;
            var second = config.AvailableInterfaces;

            Assert.Same(first, second);
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
        public void AvailableTypes_CachesComputedResult()
        {
            var config = new CanisterConfiguration();
            config.AddAssembly(typeof(CanisterConfigurationTests).Assembly);

            var first = config.AvailableTypes;
            var second = config.AvailableTypes;

            Assert.Same(first, second);
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
        public void Types_InitializesAssembliesWhenSetIsNull()
        {
            var config = new CanisterConfiguration
            {
                Assemblies = null!
            };

            var types = config.Types;

            Assert.NotNull(types);
            Assert.NotNull(config.Assemblies);
            Assert.NotEmpty(config.Assemblies);
        }

        [Fact]
        public void Types_CachesComputedResult()
        {
            var config = new CanisterConfiguration();
            config.AddAssembly(typeof(CanisterConfigurationTests).Assembly);

            var first = config.Types;
            var second = config.Types;

            Assert.Same(first, second);
        }

        [Fact]
        public void GetTypesFromAssembly_IgnoresNullLoaderExceptions()
        {
            var typeLoadException = new ReflectionTypeLoadException([typeof(string), null!], [null!]);
            var config = new ThrowingTypeLoadCanisterConfiguration(typeLoadException);

            var types = config.GetTypesFromAssembly(typeof(CanisterConfigurationTests).Assembly).ToArray();

            Assert.Single(types);
            Assert.Equal(typeof(string), types[0]);
        }

        [Fact]
        public void GetTypesFromAssembly_HandlesEmptyLoaderExceptions()
        {
            var typeLoadException = new ReflectionTypeLoadException([typeof(string)], []);
            var config = new ThrowingTypeLoadCanisterConfiguration(typeLoadException);

            var types = config.GetTypesFromAssembly(typeof(CanisterConfigurationTests).Assembly).ToArray();

            Assert.Single(types);
            Assert.Equal(typeof(string), types[0]);
        }

        [Fact]
        public void Log_DoesNotLogWhenLevelIsDisabled()
        {
            var config = new CanisterConfiguration();
            var logger = new DisabledTestLogger();

            config.UseLogger(logger, LogLevel.Trace);
            config.Log(LogLevel.Error, "disabled");
            config.Log(LogLevel.Error, new InvalidOperationException("fail"), "disabled with exception");

            Assert.Equal(0, logger.LogCallCount);
        }

        [Fact]
        public void LogSummary_ReturnsNullWhenServiceCollectionIsNull()
        {
            var config = new CanisterConfiguration();
            IServiceCollection? services = null;

            var result = config.LogSummary(services);

            Assert.Null(result);
        }

        [Fact]
        public void LogSummary_ReturnsSameServiceCollectionInstance()
        {
            var config = new CanisterConfiguration();
            IServiceCollection services = new ServiceCollection();

            var result = config.LogSummary(services);

            Assert.Same(services, result);
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

        [Fact]
        public void Types_PreservesLoadableTypes_WhenReflectionTypeLoadExceptionOccurs()
        {
            var LoaderException = new TypeLoadException("loader failure");
            var TypeLoadException = new ReflectionTypeLoadException([typeof(string), null!], [LoaderException]);
            var config = new ThrowingTypeLoadCanisterConfiguration(TypeLoadException);
            var logger = new CapturingLogger();

            config.UseLogger(logger, LogLevel.Warning);
            config.AddAssembly(typeof(CanisterConfigurationTests).Assembly);

            var Types = config.Types;

            Assert.Contains(typeof(string), Types);
            Assert.Contains(logger.Logs, x => x.Level == LogLevel.Warning && x.Exception == LoaderException);
        }

        private sealed class ThrowingTypeLoadCanisterConfiguration : CanisterConfiguration
        {
            private readonly ReflectionTypeLoadException _Exception;

            public ThrowingTypeLoadCanisterConfiguration(ReflectionTypeLoadException exception)
            {
                _Exception = exception;
            }

            internal override Type[] GetAssemblyTypes(Assembly assembly) => throw _Exception;
        }

        private sealed class CapturingLogger : ILogger
        {
            public List<(LogLevel Level, string Message, Exception? Exception)> Logs { get; } = [];

            public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullLogger.Instance.BeginScope(state);

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                Logs.Add((logLevel, formatter(state, exception), exception));
            }
        }

        private class TestLogger : ILogger
        {
            public bool WasLogged { get; private set; }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullLogger.Instance.BeginScope(state);

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
            {
                WasLogged = true;
            }
        }

        private sealed class DisabledTestLogger : ILogger
        {
            public int LogCallCount { get; private set; }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullLogger.Instance.BeginScope(state);

            public bool IsEnabled(LogLevel logLevel) => false;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                LogCallCount++;
            }
        }
    }
}