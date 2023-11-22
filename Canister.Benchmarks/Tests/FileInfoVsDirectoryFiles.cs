using BenchmarkDotNet.Attributes;
using System.IO;

namespace Canister.Benchmarks.Tests
{
    [MemoryDiagnoser, AllStatisticsColumn, RankColumn, RPlotExporter]
    public static class FileInfoVsDirectoryFiles
    {
        [Benchmark(Baseline = true)]
        public static void DirectoryFiles()
        {
            foreach (var File in Directory.EnumerateFiles(".", "*.dll"))
            { }
        }

        [Benchmark]
        public static void EnumerateFiles()
        {
            foreach (FileInfo File in new DirectoryInfo(".").EnumerateFiles("*.dll"))
            { }
        }
    }
}