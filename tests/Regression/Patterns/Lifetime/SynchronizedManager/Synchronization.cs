using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections.Generic;
using System.Threading;
#if UNITY_V4
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
#else
using Unity.Lifetime;
#endif

namespace Lifetime.Synchronization
{
    public abstract partial class Pattern
    {
#if !UNITY_V4 && !UNITY_V5
        [PatternTestMethod("SynchronizedManager.GetValue(...) blocks"), TestCategory(LIFETIME_MANAGER)]
        [DynamicData(nameof(Lifetime_Managers_Data), typeof(Lifetime.Pattern))]
        public void GetValueBlocks(LifetimeManager manager)
        {
            if (manager is not SynchronizedLifetimeManager) return;

            var scope = new LifetimeContainer();
            var value = manager.GetValue(scope);

            // Act
            Thread thread = new Thread(new ParameterizedThreadStart((c) =>
            {
                Assert.ThrowsException<TimeoutException>(() => _ = manager.GetValue(scope));
            }));

            SynchronizedLifetimeManager.ResolveTimeout = 10;
            thread.Start("1");
            thread.Join();
            SynchronizedLifetimeManager.ResolveTimeout = Timeout.Infinite;
        }
#endif

        [PatternTestMethod("SetValue(...) releases a blocks"), TestCategory(SYNCHRONIZED_MANAGER)]
        [DynamicData(nameof(Synchronized_Managers_Data), typeof(Lifetime.Pattern))]
        public void SetValueReleasesBlock(SynchronizedLifetimeManager manager)
        {
            object other = null;
            var instance = new object();
            var semaphore = new ManualResetEventSlim();
            var scope = new LifetimeContainer();
            var NoValue = manager.GetTestValue(scope);

            // Act
            Thread thread = new Thread(new ParameterizedThreadStart((c) =>
            {
                semaphore.Set();
                other = manager.GetTestValue(scope);
            }));
            
            // Wait for thread to reach semaphore and some
            thread.Start("1");
            semaphore.Wait();
            Thread.Sleep(5);
            
            // Set value and wait
            manager.SetTestValue(instance, scope);
            thread.Join();

            // Validate
            Assert.AreSame(instance, other);
            Assert.AreNotSame(NoValue, other);
        }


        [PatternTestMethod("Recover() releases a blocks"), TestCategory(SYNCHRONIZED_MANAGER)]
        [DynamicData(nameof(Synchronized_Managers_Data), typeof(Lifetime.Pattern))]
        public void RecoverReleasesBlock(SynchronizedLifetimeManager manager)
        {
            var semaphore = new ManualResetEventSlim();
            var scope = new LifetimeContainer();
            var NoValue = manager.GetTestValue(scope);
            object other = null;

            // Act
            Thread thread = new Thread(new ParameterizedThreadStart((c) =>
            {
                semaphore.Set();
                other = manager.GetTestValue(scope);
            }));

            // Wait for thread to reach semaphore and some
            thread.Start("1");
            semaphore.Wait();
            Thread.Sleep(5);

            // Set value and wait
            manager.Recover();
            thread.Join();

            // Validate
            Assert.AreSame(NoValue, other);
        }

#if !UNITY_V4 && !UNITY_V5
        [PatternTestMethod("TryGetValue(...) does not block"), TestCategory(LIFETIME_MANAGER)]
        [DynamicData(nameof(Lifetime_Managers_Data), typeof(Lifetime.Pattern))]
        public void TryGetValueDoesNotBlock(LifetimeManager manager)
        {
            var scope = new LifetimeContainer();
            var value = manager.TryGetValue(scope);
            object other = null;

            // Act
            Thread thread = new Thread(new ParameterizedThreadStart((c) =>
            {
                other = manager.TryGetValue(scope);
            }));

            thread.Start("1");
            thread.Join();

            Assert.AreSame(value, other);
        }
#endif
    }
}
