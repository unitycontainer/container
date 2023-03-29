using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Unity.Container.Tests;
using Unity.Policy;

namespace Container
{
    public partial class Defaults
    {
        #region Compliance

        [TestMethod("CompareExchange addressing as PolicySet"), TestProperty(INTERFACE, EXCHANGE)]
        public void GetOrAdd_Equal_Set()
        {
            Assert.IsNotNull(Policies.GetOrAdd(Instance, null));
            Assert.AreSame(Instance, Policies.Get(typeof(object)));
        }


        [TestMethod("CompareExchange addressing as PolicyList"), TestProperty(INTERFACE, EXCHANGE)]
        public void Exchange_Equal_List()
        {
            Assert.IsNull(Policies.CompareExchange(typeof(object), Instance, null));
            Assert.AreSame(Instance, Policies.Get(typeof(object), typeof(object)));
        }


        [PatternTestMethod(EXCHANGE_PATTERN), TestProperty(INTERFACE, EXCHANGE)]
        public void GetOrAdd_Set_null_Instance_null()
        {
            Policies.Set(null, typeof(object), (object)null);

            Assert.IsNotNull(Policies.GetOrAdd(Instance, null));
            Assert.AreSame(Instance, Policies.Get(typeof(object)));
        }


        [TestMethod("CompareExchange with not matching comparand"), TestProperty(INTERFACE, EXCHANGE)]
        public void Exchange_Empty_Nothing_Instance_object()
        {
            Assert.IsNull(Policies.CompareExchange(Instance, new object()));
            Assert.IsNull(Policies.Get(typeof(object)));
        }


        [PatternTestMethod(EXCHANGE_PATTERN), TestProperty(INTERFACE, EXCHANGE)]
        public void Exchange_Set_null_Instance_object()
        {
            Policies.Set(null, typeof(object), (object)null);

            Assert.IsNull(Policies.CompareExchange(Instance, new object()));
            Assert.IsNull(Policies.Get(typeof(object)));
        }

        #endregion


        #region Synchronization


        [TestMethod("Other is Set while synchronizing"), TestProperty(INTERFACE, EXCHANGE)]
        public void Exchange_Set_Sync_Match()
        {
            using var barrier = new Barrier(2);
            object result = null;

            Thread thread1 = new Thread(delegate ()
            {
                lock (Policies.SyncObject)
                { 
                    barrier.SignalAndWait();
                    Thread.Sleep(20);
                    Policies.CompareExchange(null, new object(), null);
                }
            })
            { Name = "1" };

            Thread thread2 = new Thread(delegate ()
            {
                barrier.SignalAndWait();
                result = Policies.CompareExchange(null, Instance, null);
            })
            { Name = "2" };

            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            Assert.IsNotNull(result);
            Assert.AreNotSame(Instance, result);
        }


        [TestMethod("Match is Set while synchronizing"), TestProperty(INTERFACE, EXCHANGE)]
        public void Exchange_Set_Sync_Other()
        {
            using var barrier = new Barrier(2);
            object result = null;

            Thread thread1 = new Thread(delegate ()
            {
                lock (Policies.SyncObject)
                {
                    barrier.SignalAndWait();
                    Thread.Sleep(20);
                }
            })
            { Name = "1" };

            Thread thread2 = new Thread(delegate ()
            {
                barrier.SignalAndWait();
                result = Policies.CompareExchange(null, Instance, null);
            })
            { Name = "2" };

            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            Assert.IsNull(result);
            Assert.AreSame(Instance, Policies.Get(typeof(object)));
        }

        #endregion
    }
}
