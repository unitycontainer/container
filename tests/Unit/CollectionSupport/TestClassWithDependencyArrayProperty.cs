namespace Unity.Tests.v5.CollectionSupport
{
    public class TestClassWithDependencyArrayProperty
    {
        [Dependency]
        public TestClass[] Dependency { get; set; }
    }
}