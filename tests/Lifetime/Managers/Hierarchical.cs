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
        public void HierarchicalTest()
        {
            // Validate nothing set
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(LifetimeContainer));
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(OtherContainer));

            // Set first
            TestManager.SetValue(TestObject, LifetimeContainer);

            // Validate set
            Assert.AreSame(TestObject, TestManager.TryGetValue(LifetimeContainer));
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(OtherContainer));

            // Set other
            var other = new object();
            TestManager.SetValue(other, OtherContainer);

            // Validate set
            Assert.AreSame(TestObject, TestManager.TryGetValue(LifetimeContainer));
            Assert.AreSame(other, TestManager.TryGetValue(OtherContainer));

            // Dispose the other
            foreach (var item in OtherContainer)
            {
                if (item is IDisposable disposable)
                    disposable.Dispose();
            }

            // Validate disposed
            Assert.AreSame(TestObject, TestManager.TryGetValue(LifetimeContainer));
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(OtherContainer));
        }

        [TestMethod]
        public void HierarchicalReverse()
        {
            // Validate nothing set
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(LifetimeContainer));
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(OtherContainer));

            // Set first
            var first = new object();
            TestManager.SetValue(first, OtherContainer);

            // Validate set
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(LifetimeContainer));
            Assert.AreSame(first, TestManager.TryGetValue(OtherContainer));

            // Set other
            TestManager.SetValue(TestObject, LifetimeContainer);

            // Validate set
            Assert.AreSame(TestObject, TestManager.TryGetValue(LifetimeContainer));
            Assert.AreSame(first, TestManager.TryGetValue(OtherContainer));

            // Dispose the other
            foreach (var item in LifetimeContainer)
            {
                if (item is IDisposable disposable)
                    disposable.Dispose();
            }

            // Validate disposed
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(LifetimeContainer));
            Assert.AreSame(first, TestManager.TryGetValue(OtherContainer));
        }

        [TestMethod]
        public void DisposeTwiceTest()
        {
            // Arrange
            TestManager.SetValue(TestObject, LifetimeContainer);
            
            // Dispose
            foreach (var item in LifetimeContainer)
            {
                if (item is IDisposable disposable)
                {
                    disposable.Dispose();
                    disposable.Dispose();
                }
            }

            // Validate nothing is set
            Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(LifetimeContainer));
        }
    }
}
