namespace Canister.Tests.Default.Services.Types
{
    public abstract class SimpleAbstractClass : ISimpleInterface
    {
        protected SimpleAbstractClass()
        {
        }

        public abstract int Value { get; }
    }
}