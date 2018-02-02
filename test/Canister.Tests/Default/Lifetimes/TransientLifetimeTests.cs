using System;
using Xunit;

namespace Canister.Tests.Default.Lifetimes
{
    public class TransientLifetimeTests
    {
        private static int DisposedCount;

        [Theory]
        [InlineData(1)]
        [InlineData(12)]
        [InlineData(4332)]
        [InlineData(123)]
        public void Copy(int value)
        {
            using (var Temp = new Canister.Default.Lifetimes.TransientLifetime(x => value, typeof(int)))
            {
                using (var Temp2 = Temp.Copy())
                {
                    Assert.NotNull(Temp2);
                    Assert.Equal(typeof(int), Temp2.ReturnType);
                    Assert.Equal(value, Temp2.Resolve(null));
                    Assert.NotSame(Temp, Temp2);
                }
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(12)]
        [InlineData(4332)]
        [InlineData(123)]
        public void Creation(int value)
        {
            using (var Temp = new Canister.Default.Lifetimes.TransientLifetime(x => value, typeof(int)))
            {
                Assert.NotNull(Temp);
                Assert.Equal(typeof(int), Temp.ReturnType);
                Assert.Equal(value, Temp.Resolve(null));
            }
        }

        [Fact]
        public void ImplementationNotSupplied()
        {
            Assert.Throws<ArgumentNullException>(() => new Canister.Default.Lifetimes.TransientLifetime(null, typeof(int)));
        }

        [Fact]
        public void ScopeTest()
        {
            using (var Temp = new Canister.Default.Lifetimes.TransientLifetime(x => new TransientTestClass(), typeof(TransientTestClass)))
            {
                Assert.NotNull(Temp);
                Assert.Equal(typeof(TransientTestClass), Temp.ReturnType);
                var Value = Temp.Resolve(null);
                Assert.IsType<TransientTestClass>(Value);
                var Value2 = Temp.Resolve(null);
                Assert.NotSame(Value, Value2);
            }
            Assert.Equal(0, DisposedCount);
        }

        private class TransientTestClass : IDisposable
        {
            public void Dispose()
            {
                ++DisposedCount;
            }
        }
    }
}