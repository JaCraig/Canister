using Canister.IoC.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Canister.Tests.Utils
{
    public class CanisterConfigurationLoggingTests
    {
        [Fact]
        public void Log_LogsAtCorrectLevel()
        {
            var config = new CanisterConfiguration();
            var logger = new TestLogger();
            config.UseLogger(logger, LogLevel.Debug);
            config.Log("Should log at default level");
            config.Log(LogLevel.Warning, "Should log at warning level");
            Assert.Contains(logger.Logs, l => l.Message.Contains("Should log at default level") && l.Level == LogLevel.Debug);
            Assert.Contains(logger.Logs, l => l.Message.Contains("Should log at warning level") && l.Level == LogLevel.Warning);
        }

        [Fact]
        public void Log_WithException_LogsException()
        {
            var config = new CanisterConfiguration();
            var logger = new TestLogger();
            config.UseLogger(logger, LogLevel.Error);
            var ex = new InvalidOperationException("fail");
            config.Log(LogLevel.Error, ex, "Error with exception");
            Assert.Contains(logger.Logs, l => l.Exception == ex && l.Message.Contains("Error with exception"));
        }

        [Fact]
        public void UseLogger_SetsLoggerAndLevel()
        {
            var config = new CanisterConfiguration();
            var logger = new TestLogger();
            config.UseLogger(logger, LogLevel.Debug);
            config.AddAssembly(typeof(CanisterConfigurationLoggingTests).Assembly);
            Assert.Contains(logger.Logs, l => l.Message.Contains("Assembly added"));
        }

        private class TestLogger : ILogger
        {
            public List<(LogLevel Level, string Message, Exception? Exception)> Logs { get; } = new();

            public IDisposable BeginScope<TState>(TState state) => null!;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                Logs.Add((logLevel, formatter(state, exception), exception));
            }
        }
    }
}