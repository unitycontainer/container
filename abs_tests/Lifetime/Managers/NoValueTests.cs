using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Lifetime;

namespace Lifetime.Managers
{
    [TestClass]
    public class NoValueTests
    {
        [TestMethod]
        public void IsNotNullTest()
        {
            // Validate
            Assert.IsNotNull(LifetimeManager.NoValue);
        }
        
        [TestMethod]
        public void HashCodeTest()
        {
            // Validate
            Assert.AreNotEqual(0, LifetimeManager.NoValue.GetHashCode());
            Assert.AreEqual(LifetimeManager.NoValue.GetHashCode(), LifetimeManager.NoValue.GetHashCode());
        }

        [TestMethod]
        public void EqualsTest()
        {
            // Validate
            Assert.IsTrue(LifetimeManager.NoValue.Equals(LifetimeManager.NoValue));
            Assert.IsFalse(LifetimeManager.NoValue.Equals(this));
        }
    }
}
