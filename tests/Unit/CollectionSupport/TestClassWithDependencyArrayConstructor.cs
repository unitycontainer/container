namespace Unit.Test.CollectionSupport
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