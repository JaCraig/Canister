using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace Canister.Benchmarks.Tests
{
    [MemoryDiagnoser]
    public class ListvsYield
    {
        [Benchmark(Baseline = true)]
        public void List()
        {
            foreach (var Item in GetList(new int[] { 1, 2, 3 }))
            { }
        }

        [Benchmark]
        public void ListNotConstructor()
        {
            foreach (var Item in GetList2(new int[] { 1, 2, 3 }))
            { }
        }

        [Benchmark]
        public void Yield()
        {
            foreach (var Item in GetYield(new int[] { 1, 2, 3 }))
            { }
        }

        private static IEnumerable<int> GetList(int[] Items)
        {
            return new List<int>(Items)
            {
                1
            };
        }

        private static IEnumerable<int> GetList2(int[] Items)
        {
            var Results = new List<int>();
            Results.AddRange(Items);
            Results.Add(1);
            return Results;
        }

        private static IEnumerable<int> GetYield(int[] Items)
        {
            for (var x = 0; x < Items.Length; ++x)
            {
                yield return Items[x];
            }
            yield return 1;
        }
    }
}