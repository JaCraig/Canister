namespace Canister.Tests.Default.Services.Types
{
    public interface IGenericInterface<T> : ISimpleInterface
        where T : SimpleAbstractClass
    {
        T Value2 { get; }
    }
}