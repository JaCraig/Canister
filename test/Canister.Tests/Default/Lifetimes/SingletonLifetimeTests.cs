using System;
using Xunit;

namespace Canister.Tests.Default.Lifetimes
{
    public class SingletonLifetimeTests
    {
        private static int DisposedCount;

        [Theory]
        [InlineData(1)]
        [InlineData(12)]
        [InlineData(4332)]
        [InlineData(123)]
        public void Copy(int value)
        {
            using (var Temp = new Canister.Default.Lifetimes.SingletonLifetime(x => value, typeof(int)))
            {
                using (var Temp2 = Temp.Copy())
                {
                    Assert.NotNull(Temp2);
                    Assert.Equal(typeof(int), Temp2.ReturnType);
                    Assert.Equal(value, Temp2.Resolve(null));
                    Assert.Same(Temp, Temp2);
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
            using (var Temp = new Canister.Default.Lifetimes.SingletonLifetime(x => value, typeof(int)))
            {
                Assert.NotNull(Temp);
                Assert.Equal(typeof(int), Temp.ReturnType);
                Assert.Equal(value, Temp.Resolve(null));
            }
        }

        [Fact]
        public void ScopeTest()
        {
            using (var Temp = new Canister.Default.Lifetimes.SingletonLifetime(x => new SingletonTestClass(), typeof(SingletonTestClass)))
            {
                Assert.NotNull(Temp);
                Assert.Equal(typeof(SingletonTestClass), Temp.ReturnType);
                var Value = Temp.Resolve(null);
                Assert.IsType(typeof(SingletonTestClass), Value);
                var Value2 = Temp.Resolve(null);
                Assert.Same(Value, Value2);
            }
            Assert.Equal(1, DisposedCount);
        }

        private class SingletonTestClass : IDisposable
        {
            public void Dispose()
            {
                ++DisposedCount;
            }
        }
    }
}