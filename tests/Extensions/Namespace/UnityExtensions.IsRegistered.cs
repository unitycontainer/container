using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;

namespace Extensions.Tests
{
    public partial class UnityExtensionsTests
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
        public void IsRegistered_Generic()
        {
            // Act
            container.IsRegistered<IUnityContainer>();

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.Name);
            Assert.ThrowsException<ArgumentNullException>(() => unity.IsRegistered<IUnityContainer>());
        }

        [TestMethod]
        public void IsRegistered_WithNameGeneric()
        {
            // Act
            container.IsRegistered<IUnityContainer>(Name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(Name, container.Name);
            Assert.ThrowsException<ArgumentNullException>(() => unity.IsRegistered<IUnityContainer>(Name));
        }

        #endregion
    }
}
