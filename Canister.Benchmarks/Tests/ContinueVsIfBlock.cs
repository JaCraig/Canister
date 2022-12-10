using BenchmarkDotNet.Attributes;

namespace Canister.Benchmarks.Tests
{
    [MemoryDiagnoser]
    public class ContinueVsIfBlock
    {
        [Params(10, 100, 1000, 10000, 100000)]
        public int Count { get; set; }

        [Benchmark]
        public void Continue()
        {
            var Temp = 0;
            for (var x = 0; x < Count; ++x)
            {
                if (Count % 2 != 0)
                    continue;
                ++Temp;
            }
        }

        [Benchmark(Baseline = true)]
        public void IfBlock()
        {
            var Temp = 0;
            for (var x = 0; x < Count; ++x)
            {
                if (Count % 2 == 0)
                {
                    ++Temp;
                }
            }
        }
    }
}