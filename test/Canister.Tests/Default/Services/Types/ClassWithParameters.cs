namespace Canister.Tests.Default.Services.Types
{
    public class ClassWithParameters : ISimpleInterface
    {
        public ClassWithParameters(SimpleClassWithDefaultConstructor classObject)
        {
            Value = classObject.Value;
        }

        public ClassWithParameters()
        {
            Value = 10;
        }

        public int Value { get; set; }
    }
}