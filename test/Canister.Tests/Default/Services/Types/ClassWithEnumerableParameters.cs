using System.Collections.Generic;
using System.Linq;

namespace Canister.Tests.Default.Services.Types
{
    public class ClassWithEnumerableParameters : ISimpleInterface
    {
        public ClassWithEnumerableParameters(IEnumerable<ISimpleInterface> classObjects)
        {
            Value = classObjects.Count();
        }

        public ClassWithEnumerableParameters()
        {
            Value = 0;
        }

        public int Value { get; set; }
    }
}