namespace Canister.Tests.Default.Services.Types
{
    public class ClassWithGenericParameters : ISimpleInterface
    {
        public ClassWithGenericParameters(IGenericInterface<SimpleClassWithAbstractParent> genericObject)
        {
            GenericObject = genericObject;
            Value = GenericObject.Value;
        }

        public IGenericInterface<SimpleClassWithAbstractParent> GenericObject { get; set; }
        public int Value { get; set; }
    }
}