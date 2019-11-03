using BenchmarkDotNet.Running;

namespace Canister.Benchmarks
{
    internal static class Program
    {
        private static void Main(string[] args) => new BenchmarkSwitcher(typeof(Program).Assembly).Run(args);
    }
}