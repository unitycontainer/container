using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Injection;

namespace Unity.Tests.v5.ContainerRegistration
{
    [TestClass]
    public class GivenContainerIntrospectionCorrectUsageFixture
    {
        private IUnityContainer container;

        [TestInitialize]
        public void Init()
        {
            container = new UnityContainer();
        }

        [TestMethod]
        public void WhenIsRegisteredIsCalledForDefaultTypeFromChildContainer()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var child = container.CreateChildContainer();

            child.RegisterType<ITypeAnotherInterface, AnotherTypeImplementation>(new InjectionConstructor("default"));
            child.RegisterType<ITypeAnotherInterface, AnotherTypeImplementation>("another", new InjectionConstructor("another"));

            var result = child.IsRegistered(typeof(ITypeAnotherInterface));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenIsRegisteredIsCalledForDefaultTypeRegisteredOnChildContainerFromParent()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var child = container.CreateChildContainer();

            child.RegisterType<ITypeAnotherInterface, AnotherTypeImplementation>(new InjectionConstructor("default"));
            child.RegisterType<ITypeAnotherInterface, AnotherTypeImplementation>("another", new InjectionConstructor("another"));

            var result = container.IsRegistered(typeof(ITypeAnotherInterface));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void WhenIsRegisteredIsCalledForDefaultType()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var result = container.IsRegistered(typeof(ITypeInterface));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenIsRegisteredIsCalledForSpecificName()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var result = container.IsRegistered(typeof(ITypeInterface), "foo");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenIsRegisteredGenericIsCalledForDefaultType()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var result = container.IsRegistered<ITypeInterface>();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenIsRegisteredGenericIsCalledForSpecificName()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var result = container.IsRegistered<ITypeInterface>("foo");

            Assert.IsTrue(result);
        }
    }
}
