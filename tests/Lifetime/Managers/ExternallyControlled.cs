using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.CompilerServices;
using Unity.Lifetime;

namespace Lifetime.Managers
{
    [TestClass]
    public class ExternallyControlled : Synchronized
    {
        protected override LifetimeManager GetManager() => new ExternallyControlledLifetimeManager();

        [TestMethod]
        public override void TryGetSetOtherContainerTest()
        {
            base.TryGetSetOtherContainerTest();

            // Validate
            Assert.AreSame(TestObject, TestManager.TryGetValue(OtherContainer));
            Assert.AreSame(TestObject, TestManager.GetValue(OtherContainer));
        }

        [TestMethod]
        public override void IsDisposedTest()
        {
            // Arrange
            var manager = TestManager as IDisposable;
            var disposable = TestObject as FakeDisposable;
            
            if (null == manager) return;

            TestManager.SetValue(TestObject, LifetimeContainer);

            Assert.IsNotNull(disposable);
            Assert.IsNotNull(manager);
            Assert.IsFalse(disposable.Disposed);

            // Act
            manager.Dispose();
            Assert.IsFalse(disposable.Disposed);
        }

        [TestMethod]
        public void CollectedTest()
        {
            // Arrange
            var manager = GetInitializedManager();

            // Act
            GC.Collect(1, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
            var instance = manager.GetValue(LifetimeContainer);

            // Validate
            if (!LifetimeManager.NoValue.Equals(instance))
                Assert.Inconclusive("GC did not collect memory, skipping test 'ExternallyControlled.CollectedTest()'");
        }

        private LifetimeManager GetInitializedManager()
        {
            // Arrange
            var instance = new object();

            // Validate set value
            TestManager.SetValue(instance, LifetimeContainer);

            Assert.AreSame(instance, TestManager.GetValue(LifetimeContainer));

            return TestManager;
        }
    }
}
