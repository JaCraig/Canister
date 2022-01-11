using Canister.Default;
using Canister.Tests.BaseClasses;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Canister.Tests.Default
{
    public class DefaultBootstrapperTests : TestBaseClass<DefaultBootstrapper>
    {
        [Fact]
        public void AddAssembly()
        {
            var Temp = GetBootstrapper();
            Temp.AddAssembly(GetType().GetTypeInfo().Assembly);
            Temp.AddAssembly(null);
            Temp.AddAssembly(System.Array.Empty<Assembly>());
        }

        [Fact]
        public void CascadeConstructor()
        {
            var Temp = GetBootstrapper();
            Temp.RegisterAll<ITestClass>();
            Temp.Register<TestClass3>();
            Temp.Register<TestClass4>();
            var Object = Temp.Resolve<TestClass4>();
            Assert.NotNull(Object);
            Assert.NotNull(Object.Class);
            Assert.Equal(2, Object.Class.Classes.Count());
        }

        [Fact]
        public void CreateDifferentScopes()
        {
            var Temp = GetBootstrapper();
            Temp.RegisterAll<ITestClass>();
            Temp.Register<TestClass3>(ServiceLifetime.Scoped);
            Temp.Register<TestClass4>(ServiceLifetime.Scoped);
            using (var NewScope = Temp.CreateScope())
            {
                var Object = NewScope.ServiceProvider.GetService(typeof(TestClass4)) as TestClass4;
                Assert.NotNull(Object);
                Assert.NotNull(Object.Class);
                Assert.Equal(2, Object.Class.Classes.Count());
                using var NewScope2 = Temp.CreateScope();
                var Object2 = NewScope2.ServiceProvider.GetService(typeof(TestClass4)) as TestClass4;
                Assert.NotNull(Object);
                Assert.NotNull(Object.Class);
                Assert.Equal(2, Object.Class.Classes.Count());
                Assert.NotSame(Object, Object2);
            }
            using (var NewScope = Temp.CreateScope())
            {
                var Object = NewScope.ServiceProvider.GetService(typeof(TestClass4)) as TestClass4;
                var Object2 = Temp.GetService(typeof(TestClass4)) as TestClass4;
                Assert.NotNull(Object);
                Assert.NotNull(Object.Class);
                Assert.Equal(2, Object.Class.Classes.Count());
                Assert.NotSame(Object, Object2);
            }
        }

        [Fact]
        public void CreateMultipleSameScope()
        {
            var Temp = GetBootstrapper();
            Temp.RegisterAll<ITestClass>();
            Temp.Register<TestClass3>(ServiceLifetime.Scoped);
            Temp.Register<TestClass4>(ServiceLifetime.Scoped);
            using var NewScope = Temp.CreateScope();
            var Object = NewScope.ServiceProvider.GetService(typeof(TestClass4)) as TestClass4;
            Assert.NotNull(Object);
            Assert.NotNull(Object.Class);
            Assert.Equal(2, Object.Class.Classes.Count());
            var Object2 = NewScope.ServiceProvider.GetService(typeof(TestClass4)) as TestClass4;
            Assert.Same(Object, Object2);
        }

        [Fact]
        public void Creation()
        {
            var Temp = GetBootstrapper();
            Assert.Equal("Default bootstrapper", Temp.Name);
        }

        [Fact]
        public void FailedResolveDependency()
        {
            var Temp = GetBootstrapper();
            Temp.RegisterAll<ITestClass>();
            Temp.Register<TestClass4>();
            var Object = Temp.Resolve(typeof(TestClass4), new TestClass4() { Class = new TestClass3() }) as TestClass4;
            Assert.NotNull(Object);
            Assert.Null(Object.Class);
        }

        [Fact]
        public void GenericTypeResolve()
        {
            var Temp = GetBootstrapper();
            Temp.Register(new TestClass { A = 13 }, ServiceLifetime.Transient);
            Temp.Register(typeof(GenericTestClass<>));
            var Object = Temp.Resolve(typeof(GenericTestClass<TestClass>), new GenericTestClass<TestClass>(null)) as GenericTestClass<TestClass>;
            Assert.NotNull(Object);
            Assert.NotNull(Object.Value);
            Assert.Equal(13, Object.Value.A);
        }

        [Fact]
        public void GetService()
        {
            var Temp = GetBootstrapper();
            Temp.RegisterAll<ITestClass>();
            Temp.Register<TestClass3>();
            Temp.Register<TestClass4>();
            var Object = Temp.GetService(typeof(TestClass4)) as TestClass4;
            Assert.NotNull(Object);
            Assert.NotNull(Object.Class);
            Assert.Equal(2, Object.Class.Classes.Count());
        }

        [Fact]
        public void IEnumerableConstructor()
        {
            var Temp = GetBootstrapper();
            Temp.RegisterAll<ITestClass>();
            Temp.Register<TestClass3>();
            var Object = Temp.Resolve<TestClass3>();
            Assert.NotNull(Object);
            Assert.Equal(2, Object.Classes.Count());
        }

        [Fact]
        public void Register()
        {
            var Temp = GetBootstrapper();
            Temp.Register(new TestClass { A = 12 });
            Assert.Equal(12, Temp.Resolve<TestClass>().A);
            Temp.Register<TestClass>();
            Assert.Equal(0, Temp.Resolve<TestClass>().A);
            Temp.Register<ITestClass, TestClass>();
            Assert.Equal(0, Temp.Resolve<ITestClass>().A);
            Temp.Register(new TestClass { A = 21 }, ServiceLifetime.Transient, "Test");
            Assert.Equal(21, Temp.Resolve<TestClass>("Test").A);
            Assert.Equal(0, Temp.Resolve<ITestClass>().A);
        }

        [Fact]
        public void RegisterAll()
        {
            var Temp = GetBootstrapper();
            Temp.RegisterAll<ITestClass>();
            Assert.NotNull(Temp.Resolve<ITestClass>());
            Assert.Equal(2, Temp.ResolveAll<ITestClass>().Count());
            Assert.NotNull(Temp.Resolve<TestClass>());
            Assert.NotNull(Temp.Resolve<TestClass2>());
        }

        [Fact]
        public void Resolve()
        {
            var Temp = GetBootstrapper();
            Temp.Register(new TestClass { A = 12 });
            Assert.Equal(12, Temp.Resolve<TestClass>().A);
            Assert.Equal(12, Temp.Resolve<TestClass>("").A);
            Assert.Equal(12, ((TestClass)Temp.Resolve(typeof(TestClass), "")).A);
        }

        [Fact]
        public void ResolveAll()
        {
            var Temp = GetBootstrapper();
            Temp.Register(new TestClass { A = 12 });
            Temp.Register(new TestClass { A = 13 }, ServiceLifetime.Transient, "A");
            Temp.Register(new TestClass { A = 14 }, ServiceLifetime.Transient, "B");
            var Objects = Temp.ResolveAll<TestClass>();
            Assert.Equal(3, Objects.Count());
            foreach (var Object in Objects)
            {
                Assert.Contains(Object.A, new int[] { 12, 13, 14 });
            }
        }

        [Fact]
        public void ResolveType()
        {
            var Temp = GetBootstrapper();
            Temp.RegisterAll<ITestClass>();
            Temp.Register<TestClass3>();
            Temp.Register<TestClass4>();
            var Object = Temp.Resolve(typeof(TestClass4), new TestClass4()) as TestClass4;
            Assert.NotNull(Object);
            Assert.NotNull(Object.Class);
            Assert.Equal(2, Object.Class.Classes.Count());
        }

        private Canister.Default.DefaultBootstrapper GetBootstrapper()
        {
            var ReturnValue = new Canister.Default.DefaultBootstrapper(new Assembly[] { typeof(DefaultBootstrapperTests).GetTypeInfo().Assembly }, new ServiceCollection());
            ReturnValue.Build();
            return ReturnValue;
        }

        protected interface ITestClass
        {
            int A { get; set; }
        }

        protected class GenericTestClass<T>
        {
            public GenericTestClass(T param1)
            {
                Value = param1;
            }

            public T Value { get; set; }
        }

        protected class TestClass : ITestClass
        {
            public int A { get; set; }
        }

        protected class TestClass2 : ITestClass
        {
            public int A { get; set; }
        }

        protected class TestClass3
        {
            public TestClass3()
            {
            }

            public TestClass3(IEnumerable<ITestClass> Classes)
            {
                this.Classes = Classes;
            }

            public IEnumerable<ITestClass> Classes { get; set; }
        }

        protected class TestClass4
        {
            public TestClass4()
            {
            }

            public TestClass4(TestClass3 Class)
            {
                this.Class = Class;
            }

            public TestClass3 Class { get; set; }
        }
    }
}