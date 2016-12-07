using Canister.Default.Services;
using Canister.Tests.Default.Services.BaseClasses;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Xunit;

namespace Canister.Tests.Default.Services
{
    public class OpenIEnumerableServiceTests : ServiceTestBase
    {
        [Fact]
        public void Create()
        {
            var TestObject = new OpenIEnumerableService(typeof(IEnumerable<>), Table, ServiceLifetime.Transient);
            Assert.Equal(typeof(IEnumerable<>), TestObject.ReturnType);
            var ReturnedList = (int[])TestObject.CreateService(typeof(IEnumerable<int>)).Create(null);
            Assert.Equal(2, ReturnedList.Length);
            Assert.Equal(1, ReturnedList[0]);
            Assert.Equal(2, ReturnedList[1]);
        }

        [Fact]
        public void CreateNoServices()
        {
            var TestObject = new OpenIEnumerableService(typeof(IEnumerable<>), Table, ServiceLifetime.Transient);
            Assert.Equal(typeof(IEnumerable<>), TestObject.ReturnType);
            var ReturnedList = (float[])TestObject.CreateService(typeof(IEnumerable<float>)).Create(null);
            Assert.Equal(1, ReturnedList.Length);
            Assert.Equal(0, ReturnedList[0]);
        }

        [Fact]
        public void Creation()
        {
            var TestObject = new OpenIEnumerableService(typeof(IEnumerable<>), Table, ServiceLifetime.Transient);
            Assert.Equal(typeof(IEnumerable<>), TestObject.ReturnType);
        }
    }
}