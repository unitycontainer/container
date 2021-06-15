namespace Unity.Tests.v5.CollectionSupport
{
    public class TestClassWithDependencyArrayMethod
    {
        public TestClass[] Dependency { get; set; }

        [InjectionMethod]
        public void Injector(TestClass[] dependency)
        {
            Dependency = dependency;
        }
    }
}