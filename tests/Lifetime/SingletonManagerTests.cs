using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Lifetime;

namespace Lifetime.Managers
{
    [TestClass]
    public class SingletonManagerTests : SynchronizedManagerTests
    {
        protected override LifetimeManager GetManager() => new SingletonLifetimeManager();

        [TestMethod]
        public override void TryGetSetOtherContainerTest()
        {
            base.TryGetSetOtherContainerTest();

            // Validate
            Assert.AreSame(TestObject, TestManager.TryGetValue(OtherContainer));
            Assert.AreSame(TestObject, TestManager.GetValue(OtherContainer));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public override void SetValueTwiceTest()
        {
            base.SetValueTwiceTest();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public override void SetDifferentValuesTwiceTest()
        {
            base.SetDifferentValuesTwiceTest();
        }
    }
}
