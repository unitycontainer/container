using System.Collections.Generic;

namespace Unity.Tests.v5.CollectionSupport
{
    public class TestClassWithDependencyEnumerableConstructor
    {
        public IEnumerable<TestClass> Dependency { get; set; }

        public TestClassWithDependencyEnumerableConstructor(IEnumerable<TestClass> dependency)
        {
            Dependency = dependency;
        }
    }
}
