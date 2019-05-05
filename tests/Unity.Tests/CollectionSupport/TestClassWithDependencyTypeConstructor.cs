namespace Unity.Tests.CollectionSupport
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