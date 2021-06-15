namespace Unity.Tests.v5.CollectionSupport
{
    public class ConfigurationTestClass
    {
        private TestClass[] arrayProperty;
        public TestClass[] ArrayProperty
        {
            get { return arrayProperty; }
            set { arrayProperty = value; }
        }

        private TestClass[] arrayMethod;
        public TestClass[] ArrayMethod
        {
            get { return arrayMethod; }
            set { arrayMethod = value; }
        }

        private TestClass[] arrayCtor;
        public TestClass[] ArrayCtor
        {
            get { return arrayCtor; }
            set { arrayCtor = value; }
        }

        public void InjectionMethod(TestClass[] arrayMethod)
        {
            ArrayMethod = arrayMethod;
        }

        [InjectionConstructor]
        public ConfigurationTestClass()
        { }

        public ConfigurationTestClass(TestClass[] arrayCtor)
        {
            ArrayCtor = arrayCtor;
        }
    }
}