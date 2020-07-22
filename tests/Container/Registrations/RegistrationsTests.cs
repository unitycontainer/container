using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity;

namespace Container.Registrations
{
    [TestClass]
    public class RegistrationsTests
    {
        protected UnityContainer Container;

        [TestInitialize]
        public virtual void InitializeTest() => Container = new UnityContainer();

        [TestMethod]
        public void IUnityContainerIsFirst() 
            => Assert.AreEqual(typeof(IUnityContainer), Container.Registrations.First().RegisteredType);

        [TestMethod]
        public void IUnityContainerAsyncIsPresent() =>
            Assert.IsTrue(Container.Registrations.Any(registration => typeof(IUnityContainerAsync) == registration.RegisteredType));

        [TestMethod]
        public void IServiceProviderIaPresent() =>
            Assert.IsTrue(Container.Registrations.Any(registration => typeof(IServiceProvider) == registration.RegisteredType));

        [TestMethod]
        public void Registrations_ToArray()
        {
            var registrations = Container.Registrations.ToArray();

            Assert.AreEqual(3, registrations.Length);
        }

        [TestMethod]
        public void EnumeratorIsCached()
        {
            // Act
            var enum1 = Container.Registrations;
            var enum2 = Container.Registrations;

            // Validate
            Assert.AreSame(enum1, enum2);
            Assert.IsTrue(enum1.SequenceEqual(enum2));
        }

        [TestMethod]
        public void CacheDiscardedOnUpdate()
        {
            // Act
            var enum1 = Container.Registrations;
            Container.RegisterInstance(this);
            var enum2 = Container.Registrations;

            // Validate
            Assert.AreNotSame(enum1, enum2);
            Assert.IsFalse(enum1.SequenceEqual(enum2));
        }

        [TestMethod]
        public void CacheDiscardedOnParentUpdate()
        {
            // Arrange
            var child = ((IUnityContainer)Container).CreateChildContainer()
                                                    .CreateChildContainer();
            var enum1 = child.Registrations;
            var enum2 = child.Registrations;

            // Act
            Container.RegisterInstance(this);
            var enum3 = child.Registrations;

            // Validate
            Assert.AreSame(enum1, enum2);
            Assert.AreNotSame(enum1, enum3);
            Assert.IsTrue(enum1.SequenceEqual(enum2));
            Assert.IsFalse(enum1.SequenceEqual(enum3));
        }

        [TestMethod]
        public void RecreatedAfterDiscarded()
        {
            // Arrange
            var enum1 = GetEnumeratorId();

            // Act
            GC.Collect(1, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            // Validate
            var enum2 = GetEnumeratorId();

            Assert.AreNotEqual(enum1, enum2);
        }


        private int GetEnumeratorId() => RuntimeHelpers.GetHashCode(Container.Registrations);
    }
}
