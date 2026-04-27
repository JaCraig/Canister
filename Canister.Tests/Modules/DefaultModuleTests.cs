using Canister.IoC.Modules;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Canister.Tests.Modules
{
    public class DefaultModuleTests
    {
        [Fact]
        public void Load_WithNullCollection_DoesNotThrow()
        {
            var module = new DefaultModule();
            module.Load(null!);
        }

        [Fact]
        public void Load_RegistersDefaultStringService()
        {
            var services = new ServiceCollection();
            var module = new DefaultModule();

            module.Load(services);

            var provider = services.BuildServiceProvider();
            Assert.Equal(string.Empty, provider.GetRequiredService<string>());
        }
    }
}