using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity;

namespace Container.Interfaces
{
    public partial class UnityInterfacesTests 
    { 
        private int GetEnumeratorId() => RuntimeHelpers.GetHashCode(Container.Registrations);


        [TestMethod]
        public void Registrations_Baseline()
        {
            var registrations = (object)Container.Registrations;

            Assert.IsNotNull(registrations);
            Assert.IsNotNull(registrations as IEnumerable<ContainerRegistration>);
        }

        [TestMethod]
        public void Registrations_IUnityContainer() 
            => Assert.AreEqual(typeof(IUnityContainer), Container.Registrations.First().RegisteredType);

        [TestMethod]
        public void Registrations_IUnityContainerAsync() =>
            Assert.IsTrue(Container.Registrations.Any(registration => typeof(IUnityContainerAsync) == registration.RegisteredType));

        [TestMethod]
        public void Registrations_IServiceProvider() =>
            Assert.IsTrue(Container.Registrations.Any(registration => typeof(IServiceProvider) == registration.RegisteredType));

        [TestMethod]
        public void Registrations_ToArray()
        {
            var registrations = Container.Registrations.ToArray();

            Assert.AreEqual(3, registrations.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Registrations_ThrowsOnChange()
        {
            var enumerator = Container.Registrations.GetEnumerator();
            
            Assert.IsTrue(enumerator.MoveNext());
            Container.RegisterInstance(this);
            enumerator.MoveNext();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Registrations_ThrowsOnParentChange()
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
        public void Registrations_CacheOnUpdate()
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
        public void Registrations_CacheOnParentUpdate()
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
        public void Registrations_Recreated()
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
    }
}
