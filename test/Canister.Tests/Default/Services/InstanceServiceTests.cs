using Canister.Default.Services;
using Canister.Tests.Default.Services.BaseClasses;
using Canister.Tests.Default.Services.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace Canister.Tests.Default.Services
{
    public class InstanceServiceTests : ServiceTestBase
    {
        public static readonly TheoryData<Type, ISimpleInterface, int> Data = new TheoryData<Type, ISimpleInterface, int>
        {
            { typeof(ISimpleInterface), new SimpleClassWithDefaultConstructor(), 1},
            {typeof(SimpleClassWithDefaultConstructor), new SimpleClassWithDefaultConstructor(), 1},
            {typeof(SimpleAbstractClass), new SimpleClassWithAbstractParent(), 2},
            {typeof(ISimpleInterface),new ClassWithParameters(new SimpleClassWithDefaultConstructor()), 1},
            {typeof(ClassWithEnumerableParameters), new ClassWithEnumerableParameters(new List<ISimpleInterface>()), 0},
            {typeof(ClassWithGenericParameters), new ClassWithGenericParameters(new GenericClass<SimpleClassWithAbstractParent>(new SimpleClassWithAbstractParent())), 2},
            {typeof(ClassWithMultipleConstructors), new ClassWithMultipleConstructors(new SimpleClassWithDefaultConstructor(),new SimpleClassWithDefaultConstructor()), 2},
            {typeof(ClassWithParameterlessPublicConstructor), new ClassWithParameterlessPublicConstructor(), 12},
            {typeof(ClassWithUnresolvableEnumerableParameters), new ClassWithUnresolvableEnumerableParameters(), 100}
        };

        [Theory]
        [MemberData("Data")]
        public void Create(Type returnType, ISimpleInterface instance, int value)
        {
            var TestObject = new InstanceService(returnType, instance, Table, ServiceLifetime.Transient);
            Assert.Equal(returnType, TestObject.ReturnType);
            var ReturnedValue = (ISimpleInterface)TestObject.Create(null);
            Assert.Equal(value, ReturnedValue.Value);
        }

        [Fact]
        public void Creation()
        {
            var TestObject = new InstanceService(typeof(int), 1, Table, ServiceLifetime.Transient);
            Assert.Equal(typeof(int), TestObject.ReturnType);
            Assert.Equal(1, TestObject.ReturnValue);
        }
    }
}