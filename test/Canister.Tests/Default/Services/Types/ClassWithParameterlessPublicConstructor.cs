namespace Canister.Tests.Default.Services.Types
{
    public class ClassWithParameterlessPublicConstructor : ISimpleInterface
    {
        public ClassWithParameterlessPublicConstructor()
            : this(12)
        {
        }

        protected ClassWithParameterlessPublicConstructor(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
    }
}