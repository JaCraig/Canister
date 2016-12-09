namespace Canister.Tests.Default.Services.Types
{
    public class GenericClass<T> : IGenericInterface<T>
        where T : SimpleAbstractClass
    {
        public GenericClass(T genericObject)
        {
            Value2 = genericObject;
            Value = genericObject.Value;
        }

        public int Value { get; set; }

        public T Value2 { get; set; }
    }
}