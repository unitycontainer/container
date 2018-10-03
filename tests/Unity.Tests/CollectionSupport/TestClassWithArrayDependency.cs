namespace Unity.Tests.v5.CollectionSupport
{
    public class TestClassWithArrayDependency
    {
        [Dependency]
        public TestClass[] Dependency { get; set; }
    }
}