using Unity;

namespace Unit.Test.CollectionSupport
{
    public class TestClassWithArrayDependency
    {
        [Dependency]
        public TestClass[] Dependency { get; set; }
    }
}