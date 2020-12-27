using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Unity.Container.Tests;

namespace Container
{
    public partial class Defaults
    {
        const string EXCHANGE = "CompareExchange";
        const string EXCHANGE_PATTERN = "{1}({2}).CompareExchange({3}, {4})";

        #region Compliance

        [TestMethod("Equal addressing with PolicySet"), TestProperty(INTERFACE, EXCHANGE)]
        public void Exchange_Equal_Set()
        {
            Assert.IsNull(Policies.CompareExchange(null, typeof(object), Instance, null));
            Assert.AreSame(Instance, Policies.Get(typeof(object)));
        }


        [TestMethod("Equality addressing with PolicyList"), TestProperty(INTERFACE, EXCHANGE)]
        public void Exchange_Equal_List()
        {
            Assert.IsNull(Policies.CompareExchange(typeof(object), typeof(object), Instance, null));
            Assert.AreSame(Instance, Policies.Get(typeof(object), typeof(object)));
        }


        [PatternTestMethod(EXCHANGE_PATTERN), TestProperty(INTERFACE, EXCHANGE)]
        public void Exchange_Set_null_Instance_null()
        {
            Policies.Set(null, typeof(object), (object)null);

            Assert.IsNull(Policies.CompareExchange(null, typeof(object), Instance, null));
            Assert.AreSame(Instance, Policies.Get(typeof(object)));
        }


        [TestMethod("Comparand does not match"), TestProperty(INTERFACE, EXCHANGE)]
        public void Exchange_Empty_Nothing_Instance_object()
        {
            Assert.IsNull(Policies.CompareExchange(null, typeof(object), Instance, new object()));
            Assert.IsNull(Policies.Get(typeof(object)));
        }


        [PatternTestMethod(EXCHANGE_PATTERN), TestProperty(INTERFACE, EXCHANGE)]
        public void Exchange_Set_null_Instance_object()
        {
            Policies.Set(null, typeof(object), (object)null);

            Assert.IsNull(Policies.CompareExchange(null, typeof(object), Instance, new object()));
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
                    Policies.CompareExchange(null, typeof(object), new object(), null);
                }
            })
            { Name = "1" };

            Thread thread2 = new Thread(delegate ()
            {
                barrier.SignalAndWait();
                result = Policies.CompareExchange(null, typeof(object), Instance, null);
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
                result = Policies.CompareExchange(null, typeof(object), Instance, null);
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
