using Canister.IoC.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Canister.Tests.ExtensionMethods
{
    public class ServiceCollectionExtensionsConcurrencyTests
    {
        [Fact]
        public async Task AddCanisterModules_CanRunConcurrentlyAcrossCollectionsAsync()
        {
            const int OuterIterations = 5;
            const int ParallelRegistrations = 24;

            for (var Iteration = 0; Iteration < OuterIterations; ++Iteration)
            {
                var Tasks = Enumerable
                    .Range(0, ParallelRegistrations)
                    .Select(Index => Task.Run(() =>
                    {
                        var services = new ServiceCollection();
                        _ = Index;
                        _ = services.AddCanisterModules(config => config.AddAssembly(typeof(ServiceCollectionExtensionsTests).Assembly));
                        using var provider = services.BuildServiceProvider();
                        return provider.GetService<ModuleTestClass>() is not null
                            && services.Exists<CanisterRegisteredFlag>();
                    }));

                var Results = await Task.WhenAll(Tasks);
                Assert.All(Results, Assert.True);
            }
        }
    }
}