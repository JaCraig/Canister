namespace Canister.Tests.Default.Services.Types
{
    public class ClassWithStringParameter : ISimpleInterface
    {
        public ClassWithStringParameter(string value)
        {
            value = string.IsNullOrEmpty(value) ? "454" : value;
            Value = int.Parse(value);
        }

        public int Value { get; }
    }
}