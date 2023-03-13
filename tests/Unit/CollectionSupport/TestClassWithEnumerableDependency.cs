using System.Collections.Generic;
using Unity;

namespace Unit.Test.CollectionSupport
{
    public class TestClassWithEnumerableDependency
    {
        [Dependency]
        public IEnumerable<TestClass> Dependency { get; set; }
    }
}
