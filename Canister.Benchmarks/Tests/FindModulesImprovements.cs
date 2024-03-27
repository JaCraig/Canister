using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Canister.Benchmarks.Tests
{
    [MemoryDiagnoser, AllStatisticsColumn, RankColumn, RPlotExporter]
    public class FindModulesImprovements
    {
        [Benchmark]
        public void HashSetVersion()
        {
            _ = FindModulesNew();
        }

        [Benchmark(Baseline = true)]
        public void Original()
        {
            _ = FindModules();
        }

        private static Assembly[] FindModules()
        {
            var EntryAssembly = Assembly.GetEntryAssembly();
            if (EntryAssembly is null)
                return Array.Empty<Assembly>();

            var ExecutingAssembly = Assembly.GetExecutingAssembly();
            if (ExecutingAssembly is null)
                return Array.Empty<Assembly>();

            var AssembliesFound = new List<Assembly>
            {
                EntryAssembly,
                ExecutingAssembly
            };
            IEnumerable<string> PathsFound = new List<string>
            {
                Path.GetDirectoryName(EntryAssembly.Location) ?? "",
                Path.GetDirectoryName(ExecutingAssembly.Location) ?? ""
            }.Distinct();

            foreach (var Path in PathsFound)
            {
                AssembliesFound.AddRange((IEnumerable<Assembly>)Directory.EnumerateFiles(Path, "*.dll", SearchOption.TopDirectoryOnly)
                    .Select(assemblyPath =>
                    {
                        try
                        {
                            return Assembly.LoadFrom(assemblyPath);
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    })
                    .Where(assembly => assembly is not null && !AssembliesFound.Contains(assembly)));
            }

            return AssembliesFound.Distinct().ToArray();
        }

        private static Assembly[] FindModulesNew()
        {
            var EntryAssembly = Assembly.GetEntryAssembly();
            if (EntryAssembly is null)
                return Array.Empty<Assembly>();

            var ExecutingAssembly = Assembly.GetExecutingAssembly();
            if (ExecutingAssembly is null)
                return Array.Empty<Assembly>();

            var AssembliesFound = new HashSet<Assembly>
            {
                EntryAssembly,
                ExecutingAssembly
            };
            var PathsFound = new HashSet<string>
            {
                Path.GetDirectoryName(EntryAssembly.Location) ?? "",
                Path.GetDirectoryName(ExecutingAssembly.Location) ?? ""
            };

            foreach (var Path in PathsFound)
            {
                foreach (var AssemblyFile in Directory.EnumerateFiles(Path, "*.dll", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        AssembliesFound.Add(Assembly.LoadFrom(AssemblyFile));
                    }
                    catch
                    {
                    }
                }
            }

            return AssembliesFound.ToArray();
        }
    }
}