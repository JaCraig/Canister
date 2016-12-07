﻿using System;
using Xunit;

namespace Canister.Tests.Default.TypeBuilders
{
    public class SingletonTypeBuilder
    {
        private static int DisposedCount;

        [Theory]
        [InlineData(1)]
        [InlineData(12)]
        [InlineData(4332)]
        [InlineData(123)]
        public void Copy(int value)
        {
            using (var Temp = new Canister.Default.TypeBuilders.SingletonTypeBuilder(x => value, typeof(int)))
            {
                using (var Temp2 = Temp.Copy())
                {
                    Assert.NotNull(Temp2);
                    Assert.Equal(typeof(int), Temp2.ReturnType);
                    Assert.Equal(value, Temp2.Create(null, new Type[0]));
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
            using (var Temp = new Canister.Default.TypeBuilders.SingletonTypeBuilder(x => value, typeof(int)))
            {
                Assert.NotNull(Temp);
                Assert.Equal(typeof(int), Temp.ReturnType);
                Assert.Equal(value, Temp.Create(null, new Type[0]));
            }
        }

        [Fact]
        public void ScopeTest()
        {
            using (var Temp = new Canister.Default.TypeBuilders.SingletonTypeBuilder(x => new SingletonTestClass(), typeof(SingletonTestClass)))
            {
                Assert.NotNull(Temp);
                Assert.Equal(typeof(SingletonTestClass), Temp.ReturnType);
                var Value = Temp.Create(null, new Type[0]);
                Assert.IsType(typeof(SingletonTestClass), Value);
                var Value2 = Temp.Create(null, new Type[0]);
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