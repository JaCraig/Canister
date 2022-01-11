using Canister.Interfaces;
using Canister.Tests.BaseClasses;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Xunit;

namespace Canister.Tests.Default
{
    public class ServiceScopeFactory : TestBaseClass<ServiceScopeFactory>
    {
        [Fact]
        public void CreateScope()
        {
            var Bootstrapper = GetBootstrapper();
            var TestObject = new Canister.Default.ServiceScopeFactory(Bootstrapper);
            var Scope = TestObject.CreateScope();
            Assert.NotNull(Scope);
            Assert.NotSame(Scope, Bootstrapper);
        }

        [Fact]
        public void Creation()
        {
            var TestObject = new Canister.Default.ServiceScopeFactory(GetBootstrapper());
            Assert.NotNull(TestObject);
        }

        private IScope GetBootstrapper()
        {
            var ReturnValue = new Canister.Default.DefaultBootstrapper(new Assembly[] { typeof(DefaultBootstrapperTests).GetTypeInfo().Assembly }, new ServiceCollection());
            ReturnValue.Build();
            return ReturnValue;
        }
    }
}