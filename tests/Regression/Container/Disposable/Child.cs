using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Container
{
    public partial class Disposal
    {
        [PatternTestMethod(SafeToDisposeMultipleTimes), TestProperty(DISPOSING, CHILD)]
        public void Child_SafeToDisposeMultipleTimes()
        {
            var container = Container.CreateChildContainer();

            container.Dispose();
            container.Dispose();
            container.Dispose();

            Assert.IsInstanceOfType(container.Resolve(typeof(IUnityContainer)), typeof(IUnityContainer));
        }

        [PatternTestMethod(InstanceDisposedOnlyOnce), TestProperty(DISPOSING, CHILD)]
        public void Child_DisposedOnlyOnce()
        {
            var container = Container.CreateChildContainer()
                                     .RegisterType<Service>(new ContainerControlledLifetimeManager());
            var service = container.Resolve<Service>();

            Assert.IsFalse(service.IsDisposed);

            container.Dispose();
            Container.Dispose();
            container.Dispose();
            Container.Dispose();

            Assert.IsTrue(service.IsDisposed);
            Assert.AreEqual(1, service.Disposals);
        }

#if !BEHAVIOR_V4
        [PatternTestMethod(InstanceAccessibleAfterDispose), TestProperty(DISPOSING, CHILD)]
        public void Child_InstanceAccessibleAfterDispose()
        {
            // Arrange
            var instance = Unresolvable.Create("root");
            var container = Container.CreateChildContainer()
                                     .RegisterInstance(instance);
            // Act
            var service = container.Resolve<Unresolvable>();
            Container.Dispose();

            Assert.AreSame(instance, service);
            Assert.AreSame(service, container.Resolve<Unresolvable>());
        }

        [PatternTestMethod(DisposableAccessibleAfterDispose), TestProperty(DISPOSING, CHILD)]
        public void Child_DisposableAccessibleAfterDispose()
        {
            // Arrange
            var container = Container.CreateChildContainer()
                                     .RegisterType<Service>(new ContainerControlledLifetimeManager());
            // Act
            var service = container.Resolve<Service>();
            Container.Dispose();

            Assert.AreSame(service, container.Resolve<Service>());
            Assert.IsTrue(service.IsDisposed);
            Assert.AreEqual(1, service.Disposals);
        }
#endif

#if !UNITY_V4
        // Unity v4 did not have ContainerControlledTransientManager

        [PatternTestMethod(SubsequentResolutionsDisposed), TestProperty(DISPOSING, CHILD)]
        public void Child_SubsequentResolutionsDisposed()
        {
            // Arrange
            var container = Container.CreateChildContainer()
                                     .RegisterType<Service>(new ContainerControlledTransientManager());
            // Act
            var befor = container.Resolve<Service>();
            Container.Dispose();

            var after = container.Resolve<Service>();

            Assert.IsTrue(befor.IsDisposed);
            Assert.IsFalse(after.IsDisposed);

            Container.Dispose();

            Assert.IsTrue(befor.IsDisposed);
            Assert.IsFalse(after.IsDisposed);

            Assert.AreEqual(1, befor.Disposals);
            Assert.AreEqual(0, after.Disposals);
        }

        [PatternTestMethod(IgnoresExceptionDuringDisposal), TestProperty(DISPOSING, CHILD)]
        public void Child_IgnoresExceptionDuringDisposal()
        {
            // Arrange
            var container = Container.CreateChildContainer()
                                     .RegisterType<ExplosiveDisposable>(new ContainerControlledTransientManager());
            // Act
            var explosive = container.Resolve<ExplosiveDisposable>();
            
            Container.Dispose();

            Assert.IsTrue(explosive.IsDisposed);
            Assert.AreEqual(1, explosive.Disposals);
        }
#endif

#if !BEHAVIOR_V4
        [PatternTestMethod(DisposesWhenDiscarded), TestProperty(DISPOSING, CHILD)]
        public void Child_DisposesWhenDiscarded()
        {
            DisposableIndicator.IsDisposed = false;

            var (weakChild, weak) = CreateResolveDiscard();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weak.IsAlive);
            Assert.IsFalse(weakChild.IsAlive);
            Assert.IsTrue(DisposableIndicator.IsDisposed);

            (WeakReference, WeakReference) CreateResolveDiscard()
            {
                var child = Container.CreateChildContainer()
                                     .RegisterType(typeof(DisposableIndicator), new ContainerControlledLifetimeManager());

                var weak = new WeakReference(child);
                var instance = new WeakReference(child.Resolve<DisposableIndicator>());

                return (weak, instance);
            }
        }
#endif
    }
}
