using System.Collections.Generic;

namespace Unity.Tests.v5.CollectionSupport
{
    public class TestClassWithDependencyInterfaceEnumerableMethod
    {
        public IEnumerable<ITestInterface> Dependency { get; set; }

        [InjectionMethod]
        public void Injector(IEnumerable<ITestInterface> dependency)
        {
            Dependency = dependency;
        }
    }

    public class TestClassWithDependencyEnumerableMethod
    {
        public IEnumerable<TestClass> Dependency { get; set; }

        [InjectionMethod]
        public void Injector(IEnumerable<TestClass> dependency)
        {
            Dependency = dependency;
        }
    }
}
