using Microsoft.Extensions.DependencyInjection;

namespace Canister.Tests.BaseClasses
{
    public abstract class TestBaseClass
    {
        protected TestBaseClass()
        {
            if (Canister.Builder.Bootstrapper == null)
            {
                new ServiceCollection().AddCanisterModules();
            }
        }
    }
}