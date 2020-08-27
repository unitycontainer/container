using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Lifetime;

namespace Lifetime.Managers
{
    [TestClass]
    public class ContainerControlled : Synchronized
    {
        protected override LifetimeManager GetManager() => new ContainerControlledLifetimeManager();

        [TestMethod]
        public override void TryGetSetOtherContainerTest()
        {
            base.TryGetSetOtherContainerTest();

            // Validate
            Assert.AreSame(TestObject, TestManager.TryGetValue(OtherContainer));
            Assert.AreSame(TestObject, TestManager.GetValue(OtherContainer));
        }
    }
}
