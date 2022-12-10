﻿using System.Linq;

namespace Canister.Tests.ErrorCases
{
    public class ServiceCollectionExtensionsExceptions
    {
        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        /// <value>The type of the object.</value>
        protected override Type? ObjectType { get; set; }

        [Fact]
        public void BufferOverflowError()
        {
            IServiceCollection Temp = GetBootstrapper();
            Temp.AddAllTransient<IService>();
            ServiceProvider Provider = Temp.BuildServiceProvider();
            System.Collections.Generic.IEnumerable<IService> TestObjects = Provider.GetServices<IService>();
            Assert.Equal(2, TestObjects.Count());
            Assert.Contains(TestObjects, x => x.Name == "MyTestService1");
            Assert.Contains(TestObjects, x => x.Name == "MyTestService2");
        }

        private IServiceCollection GetBootstrapper() => new ServiceCollection();

        public interface IService
        {
            string Name { get; }
        }

        public interface IService<T> : IService
        {
        }

        public class MyTestService1 : IService<string>
        {
            public string Name => "MyTestService1";
        }

        public class MyTestService2 : IService<string>
        {
            public string Name => "MyTestService2";
        }
    }
}