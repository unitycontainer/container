using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Lifetime;

namespace Lifetime.Managers
{
    [TestClass]
    public class ContainerTransient : LifetimeManagerTests
    {
        protected override LifetimeManager GetManager() => new ContainerControlledTransientManager();

        [TestMethod]
        public override void TryGetValueTest()
        {
            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(LifetimeContainer));

            // Act
            TestManager.SetValue(TestObject, LifetimeContainer);

            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(LifetimeContainer));
        }

        [TestMethod]
        public override void GetValueTest()
        {
            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.GetValue(LifetimeContainer));

            // Act
            TestManager.SetValue(TestObject, LifetimeContainer);

            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.GetValue(LifetimeContainer));
        }

        [TestMethod]
        public override void TryGetSetNoContainerTest()
        {
            var lifetime = new List<IDisposable>();

            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(lifetime));
            Assert.AreSame(LifetimeManager.NoValue, TestManager.GetValue(lifetime));

            // Act
            TestManager.SetValue(TestObject, lifetime);

            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(lifetime));
            Assert.AreSame(LifetimeManager.NoValue, TestManager.GetValue(lifetime));
        }

        [TestMethod]
        public override void TryGetSetOtherContainerTest()
        {
            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(LifetimeContainer));
            Assert.AreSame(LifetimeManager.NoValue, TestManager.GetValue(LifetimeContainer));

            // Act
            TestManager.SetValue(TestObject, LifetimeContainer);

            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(LifetimeContainer));
            Assert.AreSame(LifetimeManager.NoValue, TestManager.GetValue(LifetimeContainer));

            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(OtherContainer));
            Assert.AreSame(LifetimeManager.NoValue, TestManager.GetValue(OtherContainer));

            // Act
            TestManager.SetValue(TestObject, OtherContainer);

            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(LifetimeContainer));
            Assert.AreSame(LifetimeManager.NoValue, TestManager.GetValue(LifetimeContainer));
        }


        [TestMethod]
        public override void ValuesFromDifferentThreads()
        {
            TestManager.SetValue(TestObject, LifetimeContainer);

            object value1 = null;
            object value2 = null;
            object value3 = null;
            object value4 = null;

            Thread thread1 = new Thread(delegate ()
            {
                value1 = TestManager.TryGetValue(LifetimeContainer);
                value2 = TestManager.GetValue(LifetimeContainer);

            })
            { Name = "1" };

            Thread thread2 = new Thread(delegate ()
            {
                value3 = TestManager.TryGetValue(LifetimeContainer);
                value4 = TestManager.GetValue(LifetimeContainer);
            })
            { Name = "2" };

            thread1.Start();
            thread2.Start();

            thread2.Join();
            thread1.Join();

            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(LifetimeContainer));
            Assert.AreSame(LifetimeManager.NoValue, TestManager.GetValue(LifetimeContainer));

            Assert.AreSame(LifetimeManager.NoValue, value1);
            Assert.AreSame(LifetimeManager.NoValue, value2);
            Assert.AreSame(LifetimeManager.NoValue, value3);
            Assert.AreSame(LifetimeManager.NoValue, value4);
        }


        [TestMethod]
        public virtual void IsDisposedTest()
        {
            // Arrange
            var disposable = new FakeDisposable();

            Assert.IsNotNull(disposable);

            // Act
            TestManager.SetValue(disposable, LifetimeContainer);

            Assert.AreEqual(1, LifetimeContainer.Count);
        }


        #region Optimizers

        [TestMethod]
        public override void TryGetTest()
        {
            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGet(LifetimeContainer));

            // Act
            TestManager.SetValue(TestObject, LifetimeContainer);

            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGet(LifetimeContainer));
        }

        [TestMethod]
        public override void GetTest()
        {
            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.Get(LifetimeContainer));

            // Act
            TestManager.SetValue(TestObject, LifetimeContainer);

            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.Get(LifetimeContainer));
        }

        [TestMethod]
        public override void SetTest()
        {
            // Act
            TestManager.Set(TestObject, LifetimeContainer);

            // Validate
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGet(LifetimeContainer));
            Assert.AreSame(LifetimeManager.NoValue, TestManager.Get(LifetimeContainer));
        }

        #endregion

        public class FakeDisposable : IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }
    }
}
