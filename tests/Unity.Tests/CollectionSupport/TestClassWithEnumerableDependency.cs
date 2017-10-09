using Microsoft.Practices.Unity;
using System.Collections.Generic;
using Unity.Attributes;

namespace Unity.Tests.CollectionSupport
{
    public class TestClassWithEnumerableDependency
    {
        [Dependency]
        public IEnumerable<TestClass> Dependency { get; set; }
    }
}
