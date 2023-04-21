using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Storage;

namespace Container
{
    [TestClass]
    public class LifetimeContainerTest
    {
        [TestInitialize]
        public void Setup()
        {
            DisposeOrderCounter.ResetCount();
        }

        [TestMethod]
        public void CanDetermineIfLifetimeContainerContainsObject()
        {
            var container = new LifetimeContainer();
            var obj = new DisposableObject();

            container.Add(obj);

            Assert.IsTrue(container.Contains(obj));
        }

        [TestMethod]
        public void ContainerEnsuresObjectsWontBeCollected()
        {
            var container = new LifetimeContainer();
            var mdo = new DisposableObject();
            var wref = new WeakReference(mdo);

            container.Add(mdo);
            mdo = null;
            GC.Collect();

            Assert.AreEqual(1, container.Count);
            mdo = wref.Target as DisposableObject;
            Assert.IsNotNull(mdo);
            Assert.IsFalse(mdo.WasDisposed);
        }

        [TestMethod]
        public void DisposingContainerDisposesOwnedObjects()
        {
            var container = new LifetimeContainer();
            var mdo = new DisposableObject();

            container.Add(mdo);
            container.Dispose();

            Assert.IsTrue(mdo.WasDisposed);
        }

        [TestMethod]
        public void DisposingItemsFromContainerDisposesInReverseOrderAdded()
        {
            var container = new LifetimeContainer();
            var obj1 = new DisposeOrderCounter();
            var obj2 = new DisposeOrderCounter();
            var obj3 = new DisposeOrderCounter();

            container.Add(obj1);
            container.Add(obj2);
            container.Add(obj3);

            container.Dispose();

            Assert.AreEqual(1, obj3.DisposePosition);
            Assert.AreEqual(2, obj2.DisposePosition);
            Assert.AreEqual(3, obj1.DisposePosition);
        }

        [TestMethod]
        public void RemovingItemsFromContainerDoesNotDisposeThem()
        {
            var container = new LifetimeContainer();
            var mdo = new DisposableObject();

            container.Add(mdo);
            container.Remove(mdo);
            container.Dispose();

            Assert.IsFalse(mdo.WasDisposed);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CanAddAfterDisposing()
        {
            var container = new LifetimeContainer();
            var obj = new DisposableObject();

            container.Dispose();
            container.Add(obj);

            Assert.IsTrue(container.Contains(obj));
        }

        [TestMethod]
        public void RemovingNonContainedItemDoesNotThrow()
        {
            var container = new LifetimeContainer();

            container.Remove(new DisposableObject());
        }

        //[TestMethod]
        //public void ShouldDisposeAsManyAsPossibleWhenTaskExceptionIsThrown()
        //{
        //    var obj1 = new DisposableObject();
        //    var obj3 = new DisposableObject();

        //    try
        //    {
        //        using (var container = new UnityContainer())
        //        {
        //            container.RegisterInstance(nameof(obj1), obj1);
        //            var obj2 = Task.Run(async () => await Task.Delay(10000));
        //            container.RegisterInstance(nameof(obj2), obj2);
        //            container.RegisterInstance(nameof(obj3), obj3);
        //        }

        //        Assert.Fail("Exceptions should be thrown");
        //    }
        //    catch (InvalidOperationException e) when (e.Message.Contains("A task may only be disposed if it is in a completion state"))
        //    {
        //    }

        //    Assert.IsTrue(obj1.WasDisposed);
        //    Assert.IsTrue(obj3.WasDisposed);
        //}

        //[TestMethod]
        //public void ShouldDisposeAsManyAsPossibleWhenSingleExceptionIsThrown()
        //{
        //    var obj1 = new DisposableObject();
        //    var obj2 = new DisposableObjectThatThrowsOnDispose();

        //    var obj3 = new DisposableObject();

        //    try
        //    {
        //        using (var container = new UnityContainer())
        //        {
        //            container.RegisterInstance(nameof(obj1), obj1);
        //            container.RegisterInstance(nameof(obj2), obj2);
        //            container.RegisterInstance(nameof(obj3), obj3);
        //        }

        //        Assert.Fail("Exceptions should be thrown");
        //    }
        //    catch (NotImplementedException)
        //    {
        //    }

        //    Assert.IsTrue(obj1.WasDisposed);
        //    Assert.IsTrue(obj2.WasDisposed);
        //    Assert.IsTrue(obj3.WasDisposed);
        //}

        //[TestMethod]
        //public void ShouldDisposeAsManyAsPossibleWhenExceptionsAreThrown()
        //{
        //    var obj1 = new DisposableObject();
        //    var obj2 = new DisposableObjectThatThrowsOnDispose();

        //    var obj3 = new DisposableObject();
        //    var obj4 = new DisposableObjectThatThrowsOnDispose();

        //    try
        //    {
        //        using (var container = new UnityContainer())
        //        {
        //            container.RegisterInstance(nameof(obj1), obj1);
        //            container.RegisterInstance(nameof(obj2), obj2);
        //            container.RegisterInstance(nameof(obj3), obj3);
        //            container.RegisterInstance(nameof(obj4), obj4);
        //        }

        //        Assert.Fail("Exceptions should be thrown");
        //    }
        //    catch (AggregateException e)
        //    {
        //        Assert.AreEqual(2, e.InnerExceptions.Count);
        //    }

        //    Assert.IsTrue(obj1.WasDisposed);
        //    Assert.IsTrue(obj2.WasDisposed);
        //    Assert.IsTrue(obj3.WasDisposed);
        //    Assert.IsTrue(obj4.WasDisposed);
        //}

        private class DisposableObject : IDisposable
        {
            public bool WasDisposed = false;

            public virtual void Dispose()
            {
                WasDisposed = true;
            }
        }

        private class DisposeOrderCounter : IDisposable
        {
            private static int count = 0;
            public int DisposePosition;

            public static void ResetCount()
            {
                count = 0;
            }

            public void Dispose()
            {
                DisposePosition = ++count;
            }
        }

        private class DisposableObjectThatThrowsOnDispose : DisposableObject
        {
            public override void Dispose()
            {
                base.Dispose();
                throw new NotImplementedException();
            }
        }
    }
}
