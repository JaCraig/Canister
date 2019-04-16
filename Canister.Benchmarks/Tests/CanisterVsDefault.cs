using BenchmarkDotNet.Attributes;
using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Canister.Benchmarks.Tests
{
    public class CanisterVsDefault
    {
        private IBootstrapper Bootstrapper { get; set; }
        private IServiceProvider Collection { get; set; }

        [Benchmark(Baseline = true)]
        public void ConstructorInfo()
        {
            var Results = Bootstrapper.Resolve<TestClass>();
        }

        [Benchmark]
        public void Factory()
        {
            var Results = Collection.GetRequiredService<TestClass>();
        }

        [GlobalSetup]
        public void Setup()
        {
            Bootstrapper = Builder.CreateContainer(new ServiceCollection())
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