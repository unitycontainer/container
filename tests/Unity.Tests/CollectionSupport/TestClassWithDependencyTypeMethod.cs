namespace Unity.Tests.CollectionSupport
{
    public class TestClassWithDependencyTypeMethod
    {
        public TestClass[] Dependency { get; set; }

        [InjectionMethod]
        public void Injector(TestClass[] dependency)
        {
            Dependency = dependency;
        }
    }
}
