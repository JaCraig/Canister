using System;
using Xunit;

namespace Canister.Tests.Default.TypeBuilders
{
    public class GenericScopedTypeBuilder
    {
        private static int DisposedCount;

        [Theory]
        [InlineData(1)]
        [InlineData(12)]
        [InlineData(4332)]
        [InlineData(123)]
        public void Copy(int value)
        {
            using (var Temp = new Canister.Default.TypeBuilders.GenericScopedTypeBuilder((x, y) => value, typeof(int)))
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
            using (var Temp = new Canister.Default.TypeBuilders.GenericScopedTypeBuilder((x, y) => value, typeof(int)))
            {
                Assert.NotNull(Temp);
                Assert.Equal(typeof(int), Temp.ReturnType);
                Assert.Equal(value, Temp.Create(null, new Type[0]));
            }
        }

        [Fact]
        public void ScopeTest()
        {
            using (var Temp = new Canister.Default.TypeBuilders.GenericScopedTypeBuilder((x, y) => Activator.CreateInstance(typeof(GenericScopedTestClass<>).MakeGenericType(y)), typeof(GenericScopedTestClass<>)))
            {
                Assert.NotNull(Temp);
                Assert.Equal(typeof(GenericScopedTestClass<>), Temp.ReturnType);
                var Value = Temp.Create(null, new Type[] { typeof(int) });
                Assert.IsType(typeof(GenericScopedTestClass<int>), Value);
                var Value2 = Temp.Create(null, new Type[] { typeof(int) });
                Assert.Same(Value, Value2);
            }
            Assert.Equal(1, DisposedCount);
        }

        private interface IGenericScopedTestInterface<T> : IDisposable
        {
        }

        private class GenericScopedTestClass<T> : IGenericScopedTestInterface<T>
        {
            public T Value { get; set; }

            public void Dispose()
            {
                ++DisposedCount;
            }

            public override bool Equals(object obj)
            {
                return GetHashCode() == obj.GetHashCode();
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }
    }
}