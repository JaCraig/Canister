namespace Canister.Tests.Default.Services.Types
{
    public interface INotDefinedInterface
    {
        int Value { get; }
    }

    public class ClassWithOptionalParameters : ISimpleInterface
    {
        public ClassWithOptionalParameters(SimpleClassWithDefaultConstructor classObject, INotDefinedInterface exampleValue = null)
        {
            exampleValue = exampleValue ?? new NotDefinedClass();
            Value = exampleValue.Value;
        }

        public int Value { get; set; }
    }

    public class NotDefinedClass : INotDefinedInterface
    {
        public int Value => 123;
    }
}