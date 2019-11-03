using Canister.Default;
using Canister.Interfaces;
using Canister.Tests.BaseClasses;
using Xunit;

namespace Canister.Tests
{
    public class Builder : TestBaseClass
    {
        [Fact]
        public void Creation()
        {
            var Temp = Canister.Builder.Bootstrapper;
            Assert.NotNull(Temp);
            Assert.IsType<DefaultBootstrapper>(Temp);
            Assert.Equal("Default bootstrapper", Temp.Name);
            Temp.ToString();
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