using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Lifetime;
using System;
using System.Threading;

namespace Lifetime.Managers
{
    [TestClass]
    public abstract class Synchronized : LifetimeManagerTests
    {
        [TestInitialize]
        public override void SetupTest()
        {
            TestObject        = new FakeDisposable();
            TestManager       = GetManager();
            OtherContainer    = new FakeLifetimeContainer();
            LifetimeContainer = new FakeLifetimeContainer();
            SynchronizedLifetimeManager.ResolveTimeout = 100;
        }

        #region Synchronized

        [TestMethod]
        public virtual void GetSynchronizedValueTest()
        {
            var semaphor = new ManualResetEvent(false);

            new Thread(delegate ()
            {
                // Enter the lock
                _ = TestManager.GetValue(LifetimeContainer);
                semaphor.Set();

                Thread.Sleep(10);

                // Act
                TestManager.SetValue(TestObject, LifetimeContainer);
            }).Start();

            semaphor.WaitOne();
            var value = TestManager.GetValue(LifetimeContainer);

            Assert.AreSame(TestObject, value);
        }

        [TestMethod]
        public virtual void TryGetSynchronizedValueTest()
        {
            var semaphor = new ManualResetEvent(false);

            new Thread(delegate ()
            {
                // Enter the lock
                _ = TestManager.GetValue(LifetimeContainer);

                semaphor.Set();
                Thread.Sleep(10);

                // Act
                TestManager.SetValue(TestObject, LifetimeContainer);
            }).Start();

            semaphor.WaitOne();
            var value = TestManager.TryGetValue(LifetimeContainer);

            Assert.AreSame(LifetimeManager.NoValue, value);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public virtual void GetSynchronizedValueTimeoutTest()
        {
            var semaphor = new ManualResetEvent(false);

            new Thread(delegate ()
            {
                // Enter the lock
                SynchronizedLifetimeManager.ResolveTimeout = 100;
                _ = TestManager.GetValue(LifetimeContainer);
                
                semaphor.Set();
                Thread.Sleep(300);
                
                TestManager.SetValue(TestObject, LifetimeContainer);
            }).Start();

            semaphor.WaitOne();
            var value = TestManager.GetValue(LifetimeContainer);

            Assert.AreSame(LifetimeManager.NoValue, value);
        }

        #endregion


        #region Error Handling

        [TestMethod]
        public virtual void RecoverTest()
        {
            object value1 = null;
            object value2 = null;
            object value3 = null;
            var semaphor = new ManualResetEvent(false);
            var manager = GetManager();

            Thread thread1 = new Thread(delegate ()
            {
                value1 = manager.GetValue(LifetimeContainer);
                semaphor.Set();
                ((SynchronizedLifetimeManager)manager).Recover();
            });

            Thread thread2 = new Thread(delegate ()
            {
                semaphor.WaitOne();
                value2 = manager.GetValue(LifetimeContainer);
                manager.SetValue(TestObject, LifetimeContainer);
                value3 = manager.GetValue(LifetimeContainer);
            });

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.AreSame(LifetimeManager.NoValue, value1);
            Assert.AreSame(LifetimeManager.NoValue, value2);
            Assert.AreSame(TestObject, value3);
        }

        [TestMethod]
        public virtual void RecoverWithNoLockTest()
        {
            object value1 = null;
            object value2 = null;
            var manager = GetManager();

            Thread thread1 = new Thread(delegate ()
            {
                ((SynchronizedLifetimeManager)manager).Recover();
            });

            Thread thread2 = new Thread(delegate ()
            {
                value1 = manager.GetValue(LifetimeContainer);
                manager.SetValue(TestObject, LifetimeContainer);
                value2 = manager.GetValue(LifetimeContainer);
            });

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.AreSame(LifetimeManager.NoValue, value1);
            Assert.AreSame(TestObject, value2);
        }

        #endregion


        #region Optimizers

        [TestMethod]
        public override void TryGetTest()
        {
            var semaphor = new ManualResetEvent(false);

            new Thread(delegate ()
            {
                // Enter the lock
                _ = TestManager.TryGet(LifetimeContainer);
                semaphor.Set();

                Thread.Sleep(100);

                // Act
                TestManager.SetValue(TestObject, LifetimeContainer);
            }).Start();

            semaphor.WaitOne();
            SynchronizedLifetimeManager.ResolveTimeout = Timeout.Infinite;
            var value = TestManager.GetValue(LifetimeContainer);

            Assert.AreSame(LifetimeManager.NoValue, value);
        }

        [TestMethod]
        public override void GetTest()
        {
            var semaphor = new ManualResetEvent(false);

            new Thread(delegate ()
            {
                // Enter the lock
                _ = TestManager.Get(LifetimeContainer);
                semaphor.Set();

                Thread.Sleep(100);

                // Act
                TestManager.SetValue(TestObject, LifetimeContainer);
            }).Start();

            semaphor.WaitOne();
            SynchronizedLifetimeManager.ResolveTimeout = Timeout.Infinite;
            var value = TestManager.Get(LifetimeContainer);

            Assert.AreSame(TestObject, value);
        }

        [TestMethod]
        public override void SetTest()
        {
            var semaphor = new ManualResetEvent(false);

            new Thread(delegate ()
            {
                // Enter the lock
                _ = TestManager.GetValue(LifetimeContainer);
                semaphor.Set();

                Thread.Sleep(100);

                // Act
                TestManager.Set(TestObject, LifetimeContainer);
            }).Start();

            semaphor.WaitOne();
            SynchronizedLifetimeManager.ResolveTimeout = Timeout.Infinite;
            var value = TestManager.GetValue(LifetimeContainer);

            Assert.AreSame(TestObject, value);
        }

        #endregion


        #region Disposable

        [TestMethod]
        public virtual void IsDisposedTest()
        {
            // Arrange
            TestManager.SetValue(TestObject, LifetimeContainer);

            var disposable = TestObject as FakeDisposable;
            var manager = TestManager as IDisposable;

            if (null == manager) return;

            Assert.IsNotNull(disposable);
            Assert.IsNotNull(manager);
            Assert.IsFalse(disposable.Disposed);

            // Act
            manager.Dispose();
            Assert.IsTrue(disposable.Disposed);
        }

        [TestMethod]
        public virtual void DisposedUnInitializedTest()
        {
            var manager = TestManager as IDisposable;
            if (null == manager) return;

            // Act
            manager.Dispose();
        }

        public class FakeDisposable : IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }
        
        #endregion
    }
}
