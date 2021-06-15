using System.Collections.Generic;

namespace Unity.Tests.v5.CollectionSupport
{
    public class TestClassWithEnumerableDependency
    {
        [Dependency]
        public IEnumerable<TestClass> Dependency { get; set; }
    }
}
