using BenchmarkDotNet.Attributes;
using System.IO;

namespace Canister.Benchmarks.Tests
{
    [MemoryDiagnoser, AllStatisticsColumn, RankColumn, RPlotExporter]
    public class FileInfoVsDirectoryFiles
    {
        [Benchmark(Baseline = true)]
        public void DirectoryFiles()
        {
            foreach (var File in Directory.EnumerateFiles(".", "*.dll"))
            { }
        }

        [Benchmark]
        public void EnumerateFiles()
        {
            foreach (FileInfo File in new DirectoryInfo(".").EnumerateFiles("*.dll"))
            { }
        }
    }
}