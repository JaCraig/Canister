using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Canister.Tests.BaseClasses
{
    public abstract class TestBaseClass
    {
        protected TestBaseClass()
        {
            if (Canister.Builder.Bootstrapper == null)
            {
                Canister.Builder.CreateContainer(new ServiceCollection())
                   .AddAssembly(typeof(TestBaseClass).GetTypeInfo().Assembly)
                   .Build();
            }
        }
    }
}