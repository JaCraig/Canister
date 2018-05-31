using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Canister.Benchmarks.Tests
{
    public class CanisterVsDefault
    {
        private IServiceProvider Collection { get; set; }

        [Benchmark(Baseline = true)]
        public void ConstructorInfo()
        {
            var Results = Canister.Builder.Bootstrapper.Resolve<TestClass>();
        }

        [Benchmark]
        public void Factory()
        {
            var Results = Collection.GetRequiredService<TestClass>();
        }

        [GlobalSetup]
        public void Setup()
        {
            Builder.CreateContainer(new ServiceCollection())
                    .AddAssembly(typeof(CanisterVsDefault).GetTypeInfo().Assembly)
                    .Build();
            Builder.Bootstrapper.Register<TestClass>();
            Builder.Bootstrapper.Build();
            var TempProviderFactory = new DefaultServiceProviderFactory();
            var TempBuilder = TempProviderFactory.CreateBuilder(new ServiceCollection().AddTransient(typeof(TestClass)));
            Collection = TempProviderFactory.CreateServiceProvider(TempBuilder);
        }

        private class TestClass
        {
            public string Value { get; set; }
        }
    }
}