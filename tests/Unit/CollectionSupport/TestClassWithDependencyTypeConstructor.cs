namespace Unity.Tests.v5.CollectionSupport
{
    public class TestClassWithDependencyTypeConstructor
    {
        public TestClass[] Dependency { get; set; }

        public TestClassWithDependencyTypeConstructor(TestClass[] dependency)
        {
            Dependency = dependency;
        }
    }
}