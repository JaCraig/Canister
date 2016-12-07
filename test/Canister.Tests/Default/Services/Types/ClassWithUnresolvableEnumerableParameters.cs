using System.Collections.Generic;
using System.Linq;

namespace Canister.Tests.Default.Services.Types
{
    public class ClassWithUnresolvableEnumerableParameters : ISimpleInterface
    {
        public ClassWithUnresolvableEnumerableParameters(IEnumerable<IUnresolvableInterface> classObjects)
        {
            Value = classObjects.Count();
        }

        public ClassWithUnresolvableEnumerableParameters()
        {
            Value = 100;
        }

        public int Value { get; set; }
    }
}