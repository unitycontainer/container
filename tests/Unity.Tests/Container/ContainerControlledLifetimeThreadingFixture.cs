using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Unity.Lifetime;

namespace Unity.Tests.Container
{
    // Test for a race condition in the ContainerControlledLifetime
    // class.
    [TestClass]
    public class ContainerControlledLifetimeThreadingFixture
    {
        [TestMethod]
        public void SameInstanceFromMultipleThreads()
        {
            IUnityContainer container = new UnityContainer();
            container.AddExtension(new SpyExtension(new DelayStrategy(), Stage.Lifetime));
            container.RegisterType<object>(new ContainerControlledLifetimeManager());

            object result1 = null;
            object result2 = null;

            Thread thread1 = new Thread(delegate ()
            {
                result1 = container.Resolve<object>();
            });

            Thread thread2 = new Thread(delegate ()
            {
                result2 = container.Resolve<object>();
            });

            thread1.Name = "1";
            thread2.Name = "2";

            thread1.Start();
            thread2.Start();

            thread2.Join();
            thread1.Join();

            Assert.IsNotNull(result1);
            Assert.AreSame(result1, result2);
        }


        [TestMethod]
        public void ContainerControlledLifetimeDoesNotLeaveHangingLockIfBuildThrowsException()
        {
            IUnityContainer container = new UnityContainer()
                     .AddExtension(new SpyExtension(new ThrowingStrategy(), Stage.PostInitialization));
            container.RegisterType<object>(new ContainerControlledLifetimeManager());

            object result1 = null;
            object result2 = null;
            bool thread2Finished = false;

            Thread thread1 = new Thread(
                delegate()
                {
                    try
                    {
                        result1 = container.Resolve<object>();
                    }
                    catch (ResolutionFailedException)
                    {
                    }
                });

            Thread thread2 = new Thread(
                delegate()
                {
                    result2 = container.Resolve<object>();
                    thread2Finished = true;
                });

            thread1.Start();
            thread1.Join();

            // Thread1 threw an exception. However, lock should be correctly freed.
            // Run thread2, and if it finished, we're ok.

            thread2.Start();
            thread2.Join(5000);
            //thread2.Join();

            Assert.IsTrue(thread2Finished);
            Assert.IsNull(result1);
            Assert.IsNotNull(result2);
        }
    }
}
