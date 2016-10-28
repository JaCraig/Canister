using Canister.Default;
using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Canister.Tests
{
    public class Builder
    {
        [Fact]
        public void Creation()
        {
            using (IBootstrapper Temp = Canister.Builder.CreateContainer(new List<ServiceDescriptor>(), new Assembly[] { typeof(Builder).GetTypeInfo().Assembly }))
            {
                Assert.NotNull(Temp);
                Assert.IsType(typeof(DefaultBootstrapper), Temp);
                Assert.Equal("Default bootstrapper", Temp.Name);
                Temp.ToString();
            }
        }

        public class TestModule : IModule
        {
            public int Order => 1;

            public void Load(IBootstrapper bootstrapper)
            {
            }
        }
    }
}