namespace Canister.Tests.Default.Services.Types
{
    public class ClassWithOptionalBoolParameter : ISimpleInterface
    {
        public ClassWithOptionalBoolParameter(bool value = true)
        {
            this.Value = value ? 10 : 100;
        }

        public int Value { get; set; }
    }
}