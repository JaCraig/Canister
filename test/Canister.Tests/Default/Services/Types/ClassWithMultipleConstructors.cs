namespace Canister.Tests.Default.Services.Types
{
    public class ClassWithMultipleConstructors : ISimpleInterface
    {
        public ClassWithMultipleConstructors(SimpleClassWithDefaultConstructor classObject)
        {
            Value = 1;
        }

        public ClassWithMultipleConstructors(SimpleClassWithDefaultConstructor classObject, ISimpleInterface classObject2)
        {
            Value = 2;
        }

        public ClassWithMultipleConstructors()
        {
            Value = 0;
        }

        public int Value { get; set; }
    }
}