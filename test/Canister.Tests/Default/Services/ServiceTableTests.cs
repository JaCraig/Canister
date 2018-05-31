using Canister.Default.Services;
using Canister.Tests.Default.Services.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Canister.Tests.Default.Services
{
    public class ServiceTableTests
    {
        [Fact]
        public void Add()
        {
            var TestObject = new ServiceTable(new ServiceCollection(), null);
            TestObject.Add(typeof(int), "", new InstanceService(typeof(int), 1, TestObject, ServiceLifetime.Transient));
        }

        [Fact]
        public void Create()
        {
            var TestObject = new ServiceTable(new ServiceCollection(), null);
            Assert.Null(TestObject.Provider);
        }

        [Fact]
        public void CreateFromOtherTable()
        {
            var TestObject = new ServiceTable(new ServiceCollection(), null);
            TestObject.Add(typeof(int), "", new InstanceService(typeof(int), 1, TestObject, ServiceLifetime.Transient));
            var TestObject2 = new ServiceTable(TestObject);
            var ServiceObject = TestObject2.GetService(typeof(int));
            Assert.Equal(typeof(int), ServiceObject.ReturnType);
            Assert.Equal(1, ServiceObject.Create(null));
        }

        [Fact]
        public void GetService()
        {
            var TestObject = new ServiceTable(new ServiceCollection(), null);
            TestObject.Add(typeof(int), "", new InstanceService(typeof(int), 1, TestObject, ServiceLifetime.Transient));
            var ServiceObject = TestObject.GetService(typeof(int));
            Assert.Equal(typeof(int), ServiceObject.ReturnType);
        }

        [Fact]
        public void GetServiceMultipleInstaces()
        {
            var TestObject = new ServiceTable(new ServiceCollection(), null);
            TestObject.Add(typeof(int), "", new InstanceService(typeof(int), 1, TestObject, ServiceLifetime.Transient));
            TestObject.Add(typeof(int), "", new InstanceService(typeof(int), 2, TestObject, ServiceLifetime.Transient));
            var ServiceObject = TestObject.GetService(typeof(int));
            Assert.Equal(typeof(int), ServiceObject.ReturnType);
            Assert.Equal(2, ServiceObject.Create(null));
        }

        [Fact]
        public void GetServices()
        {
            var TestObject = new ServiceTable(new ServiceCollection(), null);
            TestObject.Add(typeof(int), "", new InstanceService(typeof(int), 1, TestObject, ServiceLifetime.Transient));
            TestObject.Add(typeof(int), "", new InstanceService(typeof(int), 2, TestObject, ServiceLifetime.Transient));
            var ServiceObjects = TestObject.GetServices(typeof(int));
            Assert.Equal(2, ServiceObjects.Count);
            Assert.Equal(typeof(int), ServiceObjects[0].ReturnType);
            Assert.Equal(typeof(int), ServiceObjects[1].ReturnType);
            Assert.Equal(1, ServiceObjects[0].Create(null));
            Assert.Equal(2, ServiceObjects[1].Create(null));
        }

        [Fact]
        public void Resolve()
        {
            var TestObject = new ServiceTable(new ServiceCollection(), null);
            TestObject.Add(typeof(int), "", new InstanceService(typeof(int), 1, TestObject, ServiceLifetime.Transient));
            var ServiceObject = (int)TestObject.Resolve(typeof(int));
            Assert.Equal(1, ServiceObject);
        }

        [Fact]
        public void ResolveIEnumerable()
        {
            var TestObject = new ServiceTable(new ServiceCollection(), null);
            TestObject.Add(typeof(int), "", new InstanceService(typeof(int), 1, TestObject, ServiceLifetime.Transient));
            TestObject.Add(typeof(int), "", new InstanceService(typeof(int), 2, TestObject, ServiceLifetime.Transient));
            TestObject.Add(typeof(int), "", new InstanceService(typeof(int), 3, TestObject, ServiceLifetime.Transient));
            var ServiceObject = (IEnumerable<int>)TestObject.Resolve(typeof(IEnumerable<int>));
            Assert.Equal(3, ServiceObject.Count());
            Assert.Equal(new int[] { 1, 2, 3 }, ServiceObject);
        }

        [Fact]
        public void ScopedObject()
        {
            var TestObject = new ServiceTable(new ServiceCollection(), null);
            TestObject.Add(typeof(SimpleClassWithDefaultConstructor), "", new FactoryService(typeof(SimpleClassWithDefaultConstructor), x => new SimpleClassWithDefaultConstructor(), TestObject, ServiceLifetime.Scoped));
            var ServiceObject = TestObject.Resolve(typeof(SimpleClassWithDefaultConstructor));
            using (var TestObject2 = new ServiceTable(TestObject))
            {
                var ServiceObject2 = TestObject2.Resolve(typeof(SimpleClassWithDefaultConstructor));
                Assert.NotSame(ServiceObject, ServiceObject2);
            }
            var ServiceObject3 = TestObject.Resolve(typeof(SimpleClassWithDefaultConstructor));
            Assert.Same(ServiceObject, ServiceObject3);
        }

        [Fact]
        public void ScopedObjectWithName()
        {
            var TestObject = new ServiceTable(new ServiceCollection(), null);
            TestObject.Add(typeof(SimpleClassWithDefaultConstructor), "", new FactoryService(typeof(SimpleClassWithDefaultConstructor), x => new SimpleClassWithDefaultConstructor(), TestObject, ServiceLifetime.Scoped));
            TestObject.Add(typeof(SimpleClassWithDefaultConstructor), "A", new FactoryService(typeof(SimpleClassWithDefaultConstructor), x => new SimpleClassWithDefaultConstructor(), TestObject, ServiceLifetime.Scoped));
            var ServiceObject = TestObject.Resolve(typeof(SimpleClassWithDefaultConstructor), "A");
            using (var TestObject2 = new ServiceTable(TestObject))
            {
                var ServiceObject2 = TestObject2.Resolve(typeof(SimpleClassWithDefaultConstructor), "A");
                Assert.NotSame(ServiceObject, ServiceObject2);
            }
            var ServiceObject3 = TestObject.Resolve(typeof(SimpleClassWithDefaultConstructor), "A");
            Assert.Same(ServiceObject, ServiceObject3);
            ServiceObject3 = TestObject.Resolve(typeof(SimpleClassWithDefaultConstructor));
            Assert.NotSame(ServiceObject, ServiceObject3);
        }

        [Fact]
        public void SingletonObject()
        {
            var TestObject = new ServiceTable(new ServiceCollection(), null);
            TestObject.Add(typeof(SimpleClassWithDefaultConstructor), "", new FactoryService(typeof(SimpleClassWithDefaultConstructor), x => new SimpleClassWithDefaultConstructor(), TestObject, ServiceLifetime.Singleton));
            var ServiceObject = TestObject.Resolve(typeof(SimpleClassWithDefaultConstructor));
            using (var TestObject2 = new ServiceTable(TestObject))
            {
                var ServiceObject2 = TestObject2.Resolve(typeof(SimpleClassWithDefaultConstructor));
                Assert.Same(ServiceObject, ServiceObject2);
            }
            var ServiceObject3 = TestObject.Resolve(typeof(SimpleClassWithDefaultConstructor));
            Assert.Same(ServiceObject, ServiceObject3);
        }

        [Fact]
        public void TransientObject()
        {
            var TestObject = new ServiceTable(new ServiceCollection(), null);
            TestObject.Add(typeof(SimpleClassWithDefaultConstructor), "", new FactoryService(typeof(SimpleClassWithDefaultConstructor), x => new SimpleClassWithDefaultConstructor(), TestObject, ServiceLifetime.Transient));
            var ServiceObject = TestObject.Resolve(typeof(SimpleClassWithDefaultConstructor));
            using (var TestObject2 = new ServiceTable(TestObject))
            {
                var ServiceObject2 = TestObject2.Resolve(typeof(SimpleClassWithDefaultConstructor));
                Assert.NotSame(ServiceObject, ServiceObject2);
            }
            var ServiceObject3 = TestObject.Resolve(typeof(SimpleClassWithDefaultConstructor));
            Assert.NotSame(ServiceObject, ServiceObject3);
        }
    }
}