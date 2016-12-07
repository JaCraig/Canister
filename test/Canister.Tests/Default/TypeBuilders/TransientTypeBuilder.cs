﻿using System;
using Xunit;

namespace Canister.Tests.Default.TypeBuilders
{
    public class TransientTypeBuilder
    {
        private static int DisposedCount;

        [Theory]
        [InlineData(1)]
        [InlineData(12)]
        [InlineData(4332)]
        [InlineData(123)]
        public void Copy(int value)
        {
            using (var Temp = new Canister.Default.TypeBuilders.TransientTypeBuilder(x => value, typeof(int)))
            {
                using (var Temp2 = Temp.Copy())
                {
                    Assert.NotNull(Temp2);
                    Assert.Equal(typeof(int), Temp2.ReturnType);
                    Assert.Equal(value, Temp2.Create(null, new Type[0]));
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
            using (var Temp = new Canister.Default.TypeBuilders.TransientTypeBuilder(x => value, typeof(int)))
            {
                Assert.NotNull(Temp);
                Assert.Equal(typeof(int), Temp.ReturnType);
                Assert.Equal(value, Temp.Create(null, new Type[0]));
            }
        }

        [Fact]
        public void ImplementationNotSupplied()
        {
            using (var Temp = new Canister.Default.TypeBuilders.TransientTypeBuilder(null, typeof(int)))
            {
                Assert.NotNull(Temp);
                Assert.Equal(typeof(int), Temp.ReturnType);
                Assert.Equal(0, Temp.Create(null, new Type[0]));
            }
        }

        [Fact]
        public void ScopeTest()
        {
            using (var Temp = new Canister.Default.TypeBuilders.TransientTypeBuilder(x => new TransientTestClass(), typeof(TransientTestClass)))
            {
                Assert.NotNull(Temp);
                Assert.Equal(typeof(TransientTestClass), Temp.ReturnType);
                var Value = Temp.Create(null, new Type[0]);
                Assert.IsType(typeof(TransientTestClass), Value);
                var Value2 = Temp.Create(null, new Type[0]);
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