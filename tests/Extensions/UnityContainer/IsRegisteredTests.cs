using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;

namespace Extensions.Tests
{
    public partial class UnityContainerTests
    {
        #region Registration

        [TestMethod]
        public void IsRegistered()
        {
            // Act
            container.IsRegistered(typeof(IUnityContainer));

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.Name);
            Assert.ThrowsException<ArgumentNullException>(() => unity.IsRegistered(typeof(IUnityContainer)));
            Assert.ThrowsException<ArgumentNullException>(() => container.IsRegistered(null));
        }

        [TestMethod]
        public void IsRegisteredGeneric()
        {
            // Act
            container.IsRegistered<IUnityContainer>();

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.Name);
            Assert.ThrowsException<ArgumentNullException>(() => unity.IsRegistered<IUnityContainer>());
        }

        [TestMethod]
        public void IsRegisteredWithNameGeneric()
        {
            // Act
            container.IsRegistered<IUnityContainer>(name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(name, container.Name);
            Assert.ThrowsException<ArgumentNullException>(() => unity.IsRegistered<IUnityContainer>(name));
        }

        #endregion
    }
}
