using Canister.Tests.Default.Services.BaseClasses;

namespace Canister.Tests.Default.Services
{
    public class GenericServiceTests : ServiceTestBase
    {
        //[Theory]
        //[InlineData(typeof(GenericClass<>), typeof(GenericClass<SimpleClassWithAbstractParent>), 2)]
        //[InlineData(typeof(GenericClass<>), typeof(GenericClass<SimpleAbstractClass>), 2)]
        //public void Create(Type returnType, Type closedType, int value)
        //{
        //    var TestObject = new GenericService(returnType, Table, ServiceLifetime.Transient);
        //    Assert.Equal(returnType, TestObject.ReturnType);
        //    var ReturnedValue = (ISimpleInterface)TestObject.CreateService(closedType).Create(ServiceProvider);
        //    Assert.Equal(value, ReturnedValue.Value);
        //}

        //[Fact]
        //public void Creation()
        //{
        //    var TestObject = new GenericService(typeof(int), Table, ServiceLifetime.Transient);
        //    Assert.Equal(typeof(int), TestObject.ReturnType);
        //}
    }
}