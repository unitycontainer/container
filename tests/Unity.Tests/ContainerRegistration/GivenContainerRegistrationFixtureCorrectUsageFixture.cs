using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity.Tests.v5.ContainerRegistration
{
    [TestClass]
    public class GivenContainerRegistrationFixtureCorrectUsageFixture
    {
        private IUnityContainer container;

        [TestInitialize]
        public void Init()
        {
            container = new UnityContainer();
        }

        [TestMethod]
        public void WhenRegistrationsAreRetrievedFromAContainer()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var registrations = container.Registrations;

            var count = registrations.Count();

            Assert.AreEqual(3, count);

            var @default = registrations.SingleOrDefault(c => c.Name == null &&
                                                           c.RegisteredType == typeof(ITypeInterface));

            Assert.IsNotNull(@default);
            Assert.AreEqual(typeof(TypeImplementation), @default.MappedToType);

            var foo = registrations.SingleOrDefault(c => c.Name == "foo");

            Assert.IsNotNull(foo);
            Assert.AreEqual(typeof(TypeImplementation), @default.MappedToType);
        }

        [TestMethod]
        public void WhenRegistrationsAreRetrievedFromANestedContainer()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new InjectionConstructor("foo"));

            var child = container.CreateChildContainer();

            child.RegisterType<ITypeAnotherInterface, AnotherTypeImplementation>(new InjectionConstructor("default"));
            child.RegisterType<ITypeAnotherInterface, AnotherTypeImplementation>("another", new InjectionConstructor("another"));

            var registrations = container.Registrations;

            var count = registrations.Count();

            var childCount = child.Registrations.Count();

            Assert.AreEqual(3, count);
            Assert.AreEqual(5, childCount);

            var mappedCount = child.Registrations.Where(c => c.MappedToType == typeof(AnotherTypeImplementation)).Count();

            Assert.AreEqual(2, mappedCount);
        }

        [TestMethod]
        public void WhenDefaultRegistrationsAreRetrievedFromANestedContainer()
        {
            var child = container.CreateChildContainer();

            var registrations = container.Registrations;

            var count = registrations.Count();

            var childCount = child.Registrations.Count();

            Assert.AreEqual(1, count);
            Assert.AreEqual(1, childCount);

            var mappedCount = child.Registrations.Where(c => c.MappedToType == typeof(UnityContainer)).Count();

            Assert.AreEqual(1, mappedCount);
        }

        [TestMethod]
        public void WhenRegistrationsAreRetrievedFromAContainerByLifeTimeManager()
        {
            container.RegisterType<ITypeInterface, TypeImplementation>(new PerResolveLifetimeManager(), new InjectionConstructor("default"));
            container.RegisterType<ITypeInterface, TypeImplementation>("foo", new PerResolveLifetimeManager(), new InjectionConstructor("foo"));

            var registrations = container.Registrations;

            var count = registrations.Where(c => c.LifetimeManager?.GetType() == typeof(PerResolveLifetimeManager)).Count();

            Assert.AreEqual(2, count);
        }
    }
}
