﻿using System.Reflection;
using Xunit;

namespace Canister.Tests.Default
{
    public class ServiceScopeFactory
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

        private Canister.Default.DefaultBootstrapper GetBootstrapper()
        {
            return new Canister.Default.DefaultBootstrapper(new Assembly[] { typeof(DefaultBootstrapper).GetTypeInfo().Assembly }, typeof(DefaultBootstrapper).GetTypeInfo().Assembly.GetTypes());
        }
    }
}