using System.Collections.Generic;

namespace Unity.Tests.v5.CollectionSupport
{
    public class TestClassWithDependencyInterfaceEnumerableConstructor
    {
        public IEnumerable<ITestInterface> Dependency { get; set; }

        public TestClassWithDependencyInterfaceEnumerableConstructor(IEnumerable<ITestInterface> dependency)
        {
            Dependency = dependency;
        }
    }

    public class TestClassWithDependencyEnumerableConstructor
    {
        public IEnumerable<TestClass> Dependency { get; set; }

        public TestClassWithDependencyEnumerableConstructor(IEnumerable<TestClass> dependency)
        {
            Dependency = dependency;
        }
    }
}
