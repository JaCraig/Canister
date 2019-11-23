using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Canister.Benchmarks.Tests
{
    [MemoryDiagnoser]
    public class ConstructorResolve
    {
        [Benchmark(Baseline = true)]
        public void ConstructorInfo()
        {
            _ = Builder.Bootstrapper.Resolve<TestClass>("A");
        }

        [Benchmark]
        public void Factory()
        {
            _ = Builder.Bootstrapper.Resolve<TestClass>("B");
        }

        [GlobalSetup]
        public void Setup()
        {
            Builder.CreateContainer(new ServiceCollection())
                    .AddAssembly(typeof(ConstructorResolve).GetTypeInfo().Assembly)
                    .Build();
            Builder.Bootstrapper.Register<TestClass>(name: "A");
            Builder.Bootstrapper.Register<TestClass>(_ => new TestClass(), name: "B");
            Builder.Bootstrapper.Build();
        }

        private class TestClass
        {
            public string Value { get; set; }
        }
    }
}