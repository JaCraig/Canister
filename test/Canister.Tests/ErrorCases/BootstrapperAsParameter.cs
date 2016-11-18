using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Canister.Tests.ErrorCases
{
    public class BootstrapperAsParameter
    {
        [Fact]
        public void BufferOverflowError()
        {
            using (IBootstrapper Temp = Canister.Builder.CreateContainer(new List<ServiceDescriptor>(), new Assembly[] { typeof(Builder).GetTypeInfo().Assembly }))
            {
                Temp.RegisterAll<IService>();
                var TestObjects = Temp.ResolveAll<IService>();
                Assert.Equal(2, TestObjects.Count());
                Assert.True(TestObjects.Any(x => x.Name == "MyTestService1"));
                Assert.True(TestObjects.Any(x => x.Name == "MyTestService2"));
            }
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