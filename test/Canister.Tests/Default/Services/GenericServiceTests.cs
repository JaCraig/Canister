using Canister.Default.Services;
using Canister.Tests.Default.Services.BaseClasses;
using Canister.Tests.Default.Services.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Canister.Tests.Default.Services
{
    public class GenericServiceTests : ServiceTestBase
    {
        [Theory]
        [InlineData(typeof(GenericClass<>), typeof(GenericClass<SimpleClassWithAbstractParent>), 2)]
        [InlineData(typeof(GenericClass<>), typeof(GenericClass<SimpleAbstractClass>), 2)]
        public void Create(Type returnType, Type closedType, int value)
        {
            var TestObject = new GenericService(returnType, Table, ServiceLifetime.Transient);
            Assert.Equal(returnType, TestObject.ReturnType);
            var ReturnedValue = (ISimpleInterface)TestObject.CreateService(closedType).Create(null);
            Assert.Equal(value, ReturnedValue.Value);
        }

        [Fact]
        public void Creation()
        {
            var TestObject = new GenericService(typeof(int), Table, ServiceLifetime.Transient);
            Assert.Equal(typeof(int), TestObject.ReturnType);
        }
    }
}