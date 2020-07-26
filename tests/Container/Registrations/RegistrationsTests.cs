using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
        public void Baseline()
        {
            var registrations = (object)Container.Registrations;

            Assert.IsNotNull(registrations);
            Assert.IsNotNull(registrations as IEnumerable<ContainerRegistration>);
        }

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
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThrowsOnCollctionChange()
        {
            var enumerator = Container.Registrations.GetEnumerator();
            
            Assert.IsTrue(enumerator.MoveNext());
            Container.RegisterInstance(this);
            enumerator.MoveNext();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThrowsOnParentCollctionChange()
        {
            var enumerator = ((IUnityContainer)Container)
                .CreateChildContainer()
                .Registrations
                .GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Container.RegisterInstance(this);
            enumerator.MoveNext();
        }

        [TestMethod]
        public void CacheDiscardedOnUpdate()
        {
            // Act
            var enum1 = Container.Registrations.ToArray();
            Container.RegisterInstance(this);
            var enum2 = Container.Registrations.ToArray();

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
            var enum1 = child.Registrations.ToArray();
            var enum2 = child.Registrations.ToArray();

            // Act
            Container.RegisterInstance(this);
            var enum3 = child.Registrations.ToArray();

            // Validate
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
