namespace Unity.Tests.CollectionSupport
{
    public class TestClassWithArrayDependency
    {
        [Dependency]
        public TestClass[] Dependency { get; set; }
    }
}