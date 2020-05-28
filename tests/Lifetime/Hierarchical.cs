using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Lifetime;

namespace Lifetime.Managers
{
    [TestClass]
    public class Hierarchical : Synchronized
    {
        protected override LifetimeManager GetManager() => new HierarchicalLifetimeManager();

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public override void TryGetSetNoContainerTest()
        {
            base.TryGetSetNoContainerTest();
        }

        [TestMethod]
        public override void TryGetSetOtherContainerTest()
        {
            base.TryGetSetOtherContainerTest();

            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(OtherContainer));
            Assert.AreSame(LifetimeManager.NoValue, TestManager.GetValue(OtherContainer));

            // Act
            TestManager.SetValue(TestObject, OtherContainer);

            // Validate
            Assert.AreSame(TestObject, TestManager.TryGetValue(OtherContainer));
            Assert.AreSame(TestObject, TestManager.GetValue(OtherContainer));
        }

        [TestMethod]
        public override void SetValueTwiceTest()
        {
            base.SetValueTwiceTest();
        }

        [TestMethod]
        public override void SetDifferentValuesTwiceTest()
        {
            base.SetDifferentValuesTwiceTest();
        }
    }
}
