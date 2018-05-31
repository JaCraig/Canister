using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Canister.Benchmarks.Tests
{
    public class ConstructorResolve
    {
        [Benchmark(Baseline = true)]
        public void ConstructorInfo()
        {
            var Results = Canister.Builder.Bootstrapper.Resolve<TestClass>("A");
        }

        [Benchmark]
        public void Factory()
        {
            var Results = Canister.Builder.Bootstrapper.Resolve<TestClass>("B");
        }

        [GlobalSetup]
        public void Setup()
        {
            Builder.CreateContainer(new ServiceCollection())
                    .AddAssembly(typeof(ConstructorResolve).GetTypeInfo().Assembly)
                    .Build();
            Canister.Builder.Bootstrapper.Register<TestClass>(name: "A");
            Canister.Builder.Bootstrapper.Register<TestClass>(x => new TestClass(), name: "B");
            Canister.Builder.Bootstrapper.Build();
        }

        private class TestClass
        {
            public string Value { get; set; }
        }
    }
}