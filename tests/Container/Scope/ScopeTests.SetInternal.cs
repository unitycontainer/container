using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Lifetime;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void SetInternalJustManagerTest()
        {
            // Act
            Scope.SetInternal(Manager);

            // Validate
            Assert.AreEqual(0, Scope.Version);
            Assert.AreEqual(0, Scope.Count);
            Assert.AreEqual(0, Scope.ToArray().Length);
        }

        [TestMethod]
        public void SetInternalManagerWithTypeTest()
        {
            // Act
            Scope.SetInternal(Manager, Manager.GetType());

            // Validate
            Assert.AreEqual(0, Scope.Version);
            Assert.AreEqual(1, Scope.Count);
            Assert.AreEqual(1, Scope.ToArray().Length);
        }

        [TestMethod]
        public void SetInternalAliasedManagerTest()
        {
            // Act
            Scope.SetInternal(Manager, Manager.GetType(), typeof(LifetimeManager));

            // Validate
            Assert.AreEqual(0, Scope.Version);
            Assert.AreEqual(2, Scope.Count);
            Assert.AreEqual(2, Scope.ToArray().Length);
        }

        [TestMethod]
        public void SetInternalManagerAsSameTypeTest()
        {
            // Act
            Scope.SetInternal(Manager, Manager.GetType());
            Scope.SetInternal(Manager, Manager.GetType());

            // Validate
            Assert.AreEqual(0, Scope.Version);
            Assert.AreEqual(1, Scope.Count);
            Assert.AreEqual(1, Scope.ToArray().Length);
        }
    }
}
