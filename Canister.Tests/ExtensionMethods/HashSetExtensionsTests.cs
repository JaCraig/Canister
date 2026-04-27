using Canister.IoC.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Canister.Tests.ExtensionMethods
{
    public class HashSetExtensionsTests
    {
        [Fact]
        public void GetAvailableClasses_HandlesNullAndEmptySets()
        {
            HashSet<Type>? nullSet = null;
            Assert.Empty(nullSet.GetAvailableClasses());

            var emptySet = new HashSet<Type>();
            Assert.Empty(emptySet.GetAvailableClasses());
        }

        [Fact]
        public void GetAvailableClasses_ReturnsOnlyConcreteNonOpenGenericClasses()
        {
            var types = new HashSet<Type>
            {
                typeof(string),
                typeof(AbstractClass),
                typeof(IConcreteContract),
                typeof(OpenGenericClass<>),
                typeof(OpenGenericClass<int>)
            };

            Type[] result = types.GetAvailableClasses();

            Assert.Contains(typeof(string), result);
            Assert.Contains(typeof(OpenGenericClass<int>), result);
            Assert.DoesNotContain(typeof(AbstractClass), result);
            Assert.DoesNotContain(typeof(IConcreteContract), result);
            Assert.DoesNotContain(typeof(OpenGenericClass<>), result);
        }

        [Fact]
        public void GetAvailableInterfaces_ReturnsOnlyNonOpenGenericInterfaces()
        {
            var types = new HashSet<Type>
            {
                typeof(string),
                typeof(IConcreteContract),
                typeof(IGenericContract<>),
                typeof(OpenGenericClass<>)
            };

            Type[] result = types.GetAvailableInterfaces();

            Assert.Single(result);
            Assert.Equal(typeof(IConcreteContract), result.Single());
        }

        private abstract class AbstractClass;

        private interface IConcreteContract;

        private interface IGenericContract<T>;

        private class OpenGenericClass<T>;
    }
}