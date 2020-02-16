using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Canister.Benchmarks.Tests
{
    [MemoryDiagnoser]
    public class ListvsYield
    {
        [Benchmark(Baseline = true)]
        public void List()
        {
            _ = GetList(new int[] { 1, 2, 3 });
        }

        [Benchmark]
        public void ListNotConstructor()
        {
            _ = GetList2(new int[] { 1, 2, 3 });
        }

        [Benchmark]
        public void Yield()
        {
            _ = GetYield(new int[] { 1, 2, 3 }).ToList();
        }

        private IEnumerable<int> GetList(int[] Items)
        {
            return new List<int>(Items)
            {
                1
            };
        }

        private IEnumerable<int> GetList2(int[] Items)
        {
            var Results = new List<int>();
            Results.AddRange(Items);
            Results.Add(1);
            return Results;
        }

        private IEnumerable<int> GetYield(int[] Items)
        {
            for (int x = 0; x < Items.Length; ++x)
            {
                yield return Items[x];
            }
            yield return 1;
        }
    }
}