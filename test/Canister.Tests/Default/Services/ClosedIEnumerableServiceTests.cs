using Canister.Tests.Default.Services.BaseClasses;

namespace Canister.Tests.Default.Services
{
    public class ClosedIEnumerableServiceTests : ServiceTestBase
    {
        //[Fact]
        //public void Create()
        //{
        //    var ServicesToUse = new IService[] {
        //        new InstanceService(typeof(int), 1, new ServiceTable(new ServiceCollection(), null),ServiceLifetime.Transient),
        //        new InstanceService(typeof(int), 2, new ServiceTable(new ServiceCollection(), null),ServiceLifetime.Transient)
        //    }.ToList();
        //    var TestObject = new ClosedIEnumerableService(typeof(int), ServicesToUse, Table, ServiceLifetime.Transient);
        //    Assert.Equal(2, TestObject.Services.Count);
        //    Assert.Equal(typeof(int), TestObject.ReturnType);
        //    var ReturnedList = (int[])TestObject.Create(null);
        //    Assert.Equal(2, ReturnedList.Length);
        //    Assert.Equal(1, ReturnedList[0]);
        //    Assert.Equal(2, ReturnedList[1]);
        //}

        //[Fact]
        //public void CreateNoServices()
        //{
        //    var ServicesToUse = new IService[] { }.ToList();
        //    var TestObject = new ClosedIEnumerableService(typeof(int), ServicesToUse, Table, ServiceLifetime.Transient);
        //    Assert.Empty(TestObject.Services);
        //    Assert.Equal(typeof(int), TestObject.ReturnType);
        //    var ReturnedList = (int[])TestObject.Create(null);
        //    Assert.Empty(ReturnedList);
        //}

        //[Fact]
        //public void Creation()
        //{
        //    var TestObject = new ClosedIEnumerableService(typeof(int), new List<IService>(), Table, ServiceLifetime.Transient);
        //    Assert.Empty(TestObject.Services);
        //    Assert.Equal(typeof(int), TestObject.ReturnType);
        //}
    }
}