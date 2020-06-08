using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Lifetime;

namespace Lifetime.Managers
{
    [TestClass]
    public class ExternallyControlled : Synchronized
    {
        protected override LifetimeManager GetManager() => new ExternallyControlledLifetimeManager();

        public TestContext TestContext { get; set; }

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
            var instance = new object();
            var reference = new WeakReference(instance);

            // Validate set value
            TestManager.SetValue(instance, LifetimeContainer);
            instance = TestManager.GetValue(LifetimeContainer);
            Assert.AreNotSame(LifetimeManager.NoValue, instance);

            // Act
            instance = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Validate
            if (reference.IsAlive) 
            {
                TestContext.WriteLine("GC did not collect memory, skipping test 'ExternallyControlled.CollectedTest()'");
                
                return;
            }

            instance = TestManager.GetValue(LifetimeContainer);
            Assert.AreSame(LifetimeManager.NoValue, instance);
        }
    }
}
