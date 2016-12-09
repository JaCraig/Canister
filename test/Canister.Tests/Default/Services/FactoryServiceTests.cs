using Canister.Default.Services;
using Canister.Tests.Default.Services.BaseClasses;
using Canister.Tests.Default.Services.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace Canister.Tests.Default.Services
{
    public class FactoryServiceTests : ServiceTestBase
    {
        public static readonly TheoryData<Type, Func<IServiceProvider, object>, int> Data = new TheoryData<Type, Func<IServiceProvider, object>, int>
        {
            { typeof(ISimpleInterface), x=>new SimpleClassWithDefaultConstructor(), 1},
            {typeof(SimpleClassWithDefaultConstructor), x=>new SimpleClassWithDefaultConstructor(), 1},
            {typeof(SimpleAbstractClass), x=>new SimpleClassWithAbstractParent(), 2},
            {typeof(ISimpleInterface),x=>new ClassWithParameters(new SimpleClassWithDefaultConstructor()), 1},
            {typeof(ClassWithEnumerableParameters), x=>new ClassWithEnumerableParameters(new List<ISimpleInterface>()), 0},
            {typeof(ClassWithGenericParameters), x=>new ClassWithGenericParameters(new GenericClass<SimpleClassWithAbstractParent>(new SimpleClassWithAbstractParent())), 2},
            {typeof(ClassWithMultipleConstructors), x=>new ClassWithMultipleConstructors(new SimpleClassWithDefaultConstructor(),new SimpleClassWithDefaultConstructor()), 2},
            {typeof(ClassWithParameterlessPublicConstructor), x=>new ClassWithParameterlessPublicConstructor(), 12},
            {typeof(ClassWithUnresolvableEnumerableParameters), x=>new ClassWithUnresolvableEnumerableParameters(), 100}
        };

        [Theory]
        [MemberData("Data")]
        public void Create(Type returnType, Func<IServiceProvider, object> func, int value)
        {
            var TestObject = new FactoryService(returnType, func, Table, ServiceLifetime.Transient);
            Assert.Equal(returnType, TestObject.ReturnType);
            Assert.NotNull(TestObject.Implementation);
            var ReturnedValue = (ISimpleInterface)TestObject.Create(null);
            Assert.Equal(value, ReturnedValue.Value);
        }

        [Fact]
        public void Creation()
        {
            var TestObject = new FactoryService(typeof(int), x => 0, new ServiceTable(new List<ServiceDescriptor>(), null), ServiceLifetime.Transient);
            Assert.Equal(typeof(int), TestObject.ReturnType);
            Assert.NotNull(TestObject.Implementation);
        }
    }
}