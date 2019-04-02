using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Strategies;
using Unity.Tests.v5.TestDoubles;

namespace Unity.Tests.v5.Container
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
            container.AddExtension(new SpyExtension(new DelayStrategy(), UnityBuildStage.Lifetime));
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
                .AddExtension(new SpyExtension(new ThrowingStrategy(), UnityBuildStage.PostInitialization))
                .RegisterType<object>(new ContainerControlledLifetimeManager());

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
            thread2.Join(1000);

            Assert.IsTrue(thread2Finished);
            Assert.IsNull(result1);
            Assert.IsNotNull(result2);
        }

        // A test strategy that introduces a variable delay in
        // the strategy chain to work out 
        private class DelayStrategy : BuilderStrategy
        {
            private int delayMS = 500;

            public override void PreBuildUp(ref BuilderContext context)
            {
                Thread.Sleep(this.delayMS);
                this.delayMS = this.delayMS == 0 ? 500 : 0;
            }
        }

        // Another test strategy that throws an exeception the
        // first time it is executed.
        private class ThrowingStrategy : BuilderStrategy
        {
            private bool shouldThrow = true;

            public override void PreBuildUp(ref BuilderContext context)
            {
                if (this.shouldThrow)
                {
                    this.shouldThrow = false;
                    throw new Exception("Throwing from buildup chain");
                }
            }
        }
    }
}
