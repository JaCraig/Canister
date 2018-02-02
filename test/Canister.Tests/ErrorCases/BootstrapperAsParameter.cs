using Canister.Interfaces;
using Canister.Tests.BaseClasses;
using System.Linq;
using Xunit;

namespace Canister.Tests.ErrorCases
{
    public class BootstrapperAsParameter : TestBaseClass
    {
        [Fact]
        public void BufferOverflowError()
        {
            IBootstrapper Temp = Canister.Builder.Bootstrapper;
            Temp.RegisterAll<IService>();
            var TestObjects = Temp.ResolveAll<IService>();
            Assert.Equal(2, TestObjects.Count());
            Assert.Contains(TestObjects, x => x.Name == "MyTestService1");
            Assert.Contains(TestObjects, x => x.Name == "MyTestService2");
        }

        public interface IService
        {
            string Name { get; }
        }

        public interface IService<T> : IService
        {
        }

        public class MyTestService1 : IService<string>
        {
            public MyTestService1(IBootstrapper bootstrapper)
            {
            }

            public string Name => "MyTestService1";
        }

        public class MyTestService2 : IService<string>
        {
            public string Name => "MyTestService2";
        }
    }
}