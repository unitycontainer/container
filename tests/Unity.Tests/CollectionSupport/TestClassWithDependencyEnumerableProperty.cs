using System.Collections.Generic;

namespace Unity.Tests.v5.CollectionSupport
{
    public class TestClassWithDependencyInterfaceEnumerableProperty
    {
        [Dependency]
        public IEnumerable<ITestInterface> Dependency { get; set; }
    }

    public class TestClassWithDependencyEnumerableProperty
    {
        [Dependency]
        public IEnumerable<TestClass> Dependency { get; set; }
    }
}