using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity;

namespace Container
{
    public partial class Registrations
    { 
        private int GetEnumeratorId() => RuntimeHelpers.GetHashCode(Container.Registrations);

        [TestMethod]
        public void Baseline()
        {
            var registrations = (object)Container.Registrations;

            Assert.IsNotNull(registrations);
            Assert.IsNotNull(registrations as IEnumerable<ContainerRegistration>);
        }

        [TestMethod]
        public void IUnityContainer() 
            => Assert.AreEqual(typeof(IUnityContainer), Container.Registrations.First().RegisteredType);

        [TestMethod]
        public void IUnityContainerAsync() =>
            Assert.IsTrue(Container.Registrations.Any(registration => typeof(IUnityContainerAsync) == registration.RegisteredType));

        [TestMethod]
        public void IServiceProvider() =>
            Assert.IsTrue(Container.Registrations.Any(registration => typeof(IServiceProvider) == registration.RegisteredType));
    }
}
