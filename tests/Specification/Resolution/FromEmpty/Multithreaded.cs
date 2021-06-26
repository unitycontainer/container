using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System.Threading;
using System.Threading.Tasks;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Resolution
{
    public partial class FromEmpty
    {


        [TestMethod("Unregistered from different threads"), TestProperty(RESOLVING, nameof(FromEmpty))]
        public void Unregistered_Multithreaded()
        {
            var barrier = new Barrier(2);
            var storage = new Service[2];

            // Act
            Parallel.Invoke(MaxDegreeOfParallelism,
                () =>  {
                    barrier.SignalAndWait();
                    storage[0] = Container.Resolve<Service>(); 
                },
                () => {
                    barrier.SignalAndWait();
                    storage[1] = Container.Resolve<Service>();
                });

            // Validate
            Assert.IsFalse(Container.IsRegistered<Service>());

            Assert.IsNotNull(storage[0]);
            Assert.IsNotNull(storage[1]);

            // All different instances
            Assert.AreNotSame(storage[0], storage[1]);

            // All different threads
            Assert.AreNotEqual(storage[0].ThreadId, storage[1].ThreadId);
        }


    }
}
