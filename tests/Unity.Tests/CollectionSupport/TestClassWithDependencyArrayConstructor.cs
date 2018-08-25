namespace Unity.Tests.v5.CollectionSupport
{
    public class TestClassWithDependencyArrayConstructor
    {
        public TestClass[] Dependency { get; set; }

        public TestClassWithDependencyArrayConstructor(TestClass[] dependency)
        {
            Dependency = dependency;
        }
    }
}