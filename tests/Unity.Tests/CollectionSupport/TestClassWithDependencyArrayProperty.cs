namespace Unity.Tests.CollectionSupport
{
    public class TestClassWithDependencyArrayProperty
    {
        [Dependency]
        public TestClass[] Dependency { get; set; }
    }
}