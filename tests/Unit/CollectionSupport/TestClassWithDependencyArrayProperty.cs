using Unity;

namespace Unit.Test.CollectionSupport
{
    public class TestClassWithDependencyArrayProperty
    {
        [Dependency]
        public TestClass[] Dependency { get; set; }
    }
}