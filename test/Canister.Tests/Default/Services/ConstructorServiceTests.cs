using Canister.Default.Services;
using Canister.Tests.Default.Services.BaseClasses;
using Canister.Tests.Default.Services.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace Canister.Tests.Default.Services
{
    public class ConstructorServiceTests : ServiceTestBase
    {
        [Fact]
        public void ClassMustHaveConstructor()
        {
            var TestObject = new ConstructorService(typeof(ClassWithNoPublicConstructors), typeof(ClassWithNoPublicConstructors), Table, ServiceLifetime.Transient);
            Assert.Equal(typeof(ClassWithNoPublicConstructors), TestObject.ReturnType);
            Assert.Equal(typeof(ClassWithNoPublicConstructors), TestObject.ImplementationType);
            var ReturnedValue = (ISimpleInterface)TestObject.Create(null);
            Assert.Null(ReturnedValue);
        }

        [Theory]
        [InlineData(typeof(ISimpleInterface), typeof(SimpleClassWithDefaultConstructor), 1)]
        [InlineData(typeof(SimpleClassWithDefaultConstructor), typeof(SimpleClassWithDefaultConstructor), 1)]
        [InlineData(typeof(SimpleAbstractClass), typeof(SimpleClassWithAbstractParent), 2)]
        [InlineData(typeof(ISimpleInterface), typeof(ClassWithParameters), 1)]
        [InlineData(typeof(ClassWithEnumerableParameters), typeof(ClassWithEnumerableParameters), 2)]
        [InlineData(typeof(ClassWithGenericParameters), typeof(ClassWithGenericParameters), 2)]
        [InlineData(typeof(ClassWithMultipleConstructors), typeof(ClassWithMultipleConstructors), 2)]
        [InlineData(typeof(ClassWithParameterlessPublicConstructor), typeof(ClassWithParameterlessPublicConstructor), 12)]
        [InlineData(typeof(ClassWithUnresolvableEnumerableParameters), typeof(ClassWithUnresolvableEnumerableParameters), 100)]
        public void Create(Type returnType, Type implementationType, int value)
        {
            var TestObject = new ConstructorService(returnType, implementationType, Table, ServiceLifetime.Transient);
            Assert.Equal(returnType, TestObject.ReturnType);
            Assert.Equal(implementationType, TestObject.ImplementationType);
            var ReturnedValue = (ISimpleInterface)TestObject.Create(null);
            Assert.Equal(value, ReturnedValue.Value);
        }

        [Fact]
        public void Creation()
        {
            var TestObject = new ConstructorService(typeof(int), typeof(int), new ServiceTable(new List<ServiceDescriptor>(), null), ServiceLifetime.Transient);
            Assert.Equal(typeof(int), TestObject.ReturnType);
            Assert.Equal(typeof(int), TestObject.ImplementationType);
        }
    }
}