
using Unity.Tests.v5.TestDoubles;

namespace Unity.Tests.v5.TestObjects
{
    internal class ObjectWithAmbiguousMarkedConstructor
    {
        public ObjectWithAmbiguousMarkedConstructor()
        {
        }

        public ObjectWithAmbiguousMarkedConstructor(int first, string second, float third)
        {
        }

        [InjectionConstructor]
        public ObjectWithAmbiguousMarkedConstructor(string first, string second, int third)
        {
        }
    }
}
