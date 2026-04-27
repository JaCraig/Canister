using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace Canister.Tests.ExtensionMethods
{
    public class ServiceCollectionExtensionsIntegrationTests
    {
        [Fact]
        public void AddCanisterModules_WithAssemblyParameter_RegistersAttributedTypes()
        {
            var services = new ServiceCollection();
            services.AddCanisterModules(typeof(ServiceCollectionAddByAttribute).Assembly);

            var provider = services.BuildServiceProvider();

            Assert.NotNull(provider.GetService<IInterfaceToAdd2>());
            Assert.Equal(2, provider.GetServices<IInterfaceToAdd>().Count());
        }

        [Fact]
        public void AddCanisterModules_WhenCalledTwice_DoesNotDuplicateAttributedRegistrations()
        {
            var services = new ServiceCollection();
            services.AddCanisterModules(typeof(ServiceCollectionAddByAttribute).Assembly);
            services.AddCanisterModules(typeof(ServiceCollectionAddByAttribute).Assembly);

            var provider = services.BuildServiceProvider();

            Assert.Equal(2, provider.GetServices<IInterfaceToAdd>().Count());
        }
    }
}