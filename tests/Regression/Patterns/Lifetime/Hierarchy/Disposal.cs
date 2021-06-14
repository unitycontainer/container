using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections.Generic;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Lifetime.Hierarchies
{
    public abstract partial class Pattern
    {
        protected const string DISPOSE_NAME_FORMAT = "Setup in {0}, imported in {1}, {2} is {4}";

        #region Not Registered

        [PatternTestMethod(DISPOSE_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_NONE)]
        [DynamicData(nameof(Disposable_Managers_Data))]
        public void None_Root_Root_root_disposed(string name, LifetimeManagerFactoryDelegate factory, params Action<SingletonService>[] asserts)
        {
            var unregistered = Container.Resolve<SingletonService>();

            Container.Dispose();

            Assert.IsFalse(unregistered.IsDisposed, $"{nameof(unregistered)} should not be disposed");
        }


        [PatternTestMethod(DISPOSE_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_NONE)]
        [DynamicData(nameof(Disposable_Managers_Data))]
        public void None_Child_Root_child_disposed(string name, LifetimeManagerFactoryDelegate factory, params Action<SingletonService>[] asserts)
        {
            var weak = new WeakReference(Container);
            var (weakChild, instance) = CreateResolveAndDispose(factory);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weak.IsAlive);
            Assert.IsFalse(weakChild.IsAlive);
            Assert.IsFalse(instance.IsDisposed, $"{nameof(instance)} should not be disposed");
        }


        [PatternTestMethod(DISPOSE_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_NONE)]
        [DynamicData(nameof(Disposable_Managers_Data))]
        public void None_Child_Child_child_disposed(string name, LifetimeManagerFactoryDelegate factory, params Action<SingletonService>[] asserts)
        {
            var (weakChild, instance) = CreateResolveAndDispose(factory);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weakChild.IsAlive);
            Assert.IsFalse(instance.IsDisposed, $"{nameof(instance)} should not be disposed");

        }

        #endregion


        #region Register In Root

        [PatternTestMethod(DISPOSE_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Disposable_Managers_Data))]
        public void Root_Root_Root_root_disposed(string name, LifetimeManagerFactoryDelegate factory, params Action<SingletonService>[] asserts)
        {
            var (weak, instance) = CreateResolveAndDispose(factory);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weak.IsAlive);
            foreach (var assert in asserts) assert(instance);

            (WeakReference, SingletonService) CreateResolveAndDispose(LifetimeManagerFactoryDelegate factory)
            {
                var weak = new WeakReference(Container);

                Container.RegisterType(typeof(SingletonService), factory());

                var instance = Container.Resolve<SingletonService>();

                Container.Dispose();
                Container = new UnityContainer(); // Must replace to change reference

                return (weak, instance);
            }
        }

#if !BEHAVIOR_V4
        [PatternTestMethod(DISPOSE_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Disposable_Managers_Data))]
        public void Root_Root_Root_root_discarded(string name, LifetimeManagerFactoryDelegate factory, params Action<SingletonService>[] asserts)
        {
            var (weak, instance) = CreateResolveAndDispose(factory);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weak.IsAlive);
            foreach (var assert in asserts) assert(instance);

            (WeakReference, SingletonService) CreateResolveAndDispose(LifetimeManagerFactoryDelegate factory)
            {
                var weak = new WeakReference(Container);

                Container.RegisterType(typeof(SingletonService), factory());

                var instance = Container.Resolve<SingletonService>();

                Container = new UnityContainer(); // Must replace to change reference

                return (weak, instance);
            }
        }
#endif

        [PatternTestMethod(DISPOSE_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Disposable_Managers_Data))]
        public void Root_Child_Root_root_disposed(string name, LifetimeManagerFactoryDelegate factory, params Action<SingletonService>[] asserts)
        {
            var (weak, instance) = CreateResolveAndDispose(factory);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weak.IsAlive);
            foreach (var assert in asserts) assert(instance);

            (WeakReference, SingletonService) CreateResolveAndDispose(LifetimeManagerFactoryDelegate factory)
            {
                var weak = new WeakReference(Container);

                Container.RegisterType(typeof(SingletonService), factory());

                var instance = Container.CreateChildContainer()
                                        .Resolve<SingletonService>();
                Container.Dispose();
                Container = new UnityContainer(); // Must replace to change reference

                return (weak, instance);
            }
        }

#if !BEHAVIOR_V4
        [PatternTestMethod(DISPOSE_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Disposable_Managers_Data))]
        public void Root_Child_Root_root_discarded(string name, LifetimeManagerFactoryDelegate factory, params Action<SingletonService>[] asserts)
        {
            var (weak, instance) = CreateResolveAndDispose(factory);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weak.IsAlive);
            foreach (var assert in asserts) assert(instance);

            (WeakReference, SingletonService) CreateResolveAndDispose(LifetimeManagerFactoryDelegate factory)
            {
                var weak = new WeakReference(Container);

                Container.RegisterType(typeof(SingletonService), factory());

                var instance = Container.CreateChildContainer()
                                        .Resolve<SingletonService>();
                Container = new UnityContainer(); // Must replace to change reference

                return (weak, instance);
            }
        }


        [PatternTestMethod(DISPOSE_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Disposable_Managers_Data))]
        public void Root_Child_Child_child_disposed(string name, LifetimeManagerFactoryDelegate factory, params Action<SingletonService>[] asserts)
        {
            Container.RegisterType(typeof(SingletonService), factory());

            var (weak, instance) = ResolveAndDispose();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weak.IsAlive);
            if (instance.Default is null)
                Disposed_True(instance);
            else
                Disposed_False(instance);

            (WeakReference, SingletonService) ResolveAndDispose()
            {
                var child = Container.CreateChildContainer();
                var value = child.Resolve<SingletonService>();

                child.Dispose();

                return (new WeakReference(child), value);
            }
        }

        [PatternTestMethod(DISPOSE_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_ROOT)]
        [DynamicData(nameof(Disposable_Managers_Data))]
        public void Root_Child_Child_child_discarded(string name, LifetimeManagerFactoryDelegate factory, params Action<SingletonService>[] asserts)
        {
            Container.RegisterType(typeof(SingletonService), factory());

            var (weak, instance) = ResolveAndDiscard();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weak.IsAlive);
            if (instance.Default is null)
                Disposed_True(instance);
            else
                Disposed_False(instance);

            (WeakReference, SingletonService) ResolveAndDiscard()
            {
                var child = Container.CreateChildContainer();
                var value = child.Resolve<SingletonService>();

                return (new WeakReference(child), value);
            }
        }
#endif


        #endregion


        #region Register In Child

        [PatternTestMethod(DISPOSE_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_CHILD)]
        [DynamicData(nameof(Disposable_Managers_Data))]
        public void Child_Child_Child_child_disposed(string name, LifetimeManagerFactoryDelegate factory, params Action<SingletonService>[] asserts)
        {
            var (weakChild, instance) = CreateResolveDispose(factory);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weakChild.IsAlive);
            foreach (var assert in asserts) assert(instance);

            (WeakReference, SingletonService) CreateResolveDispose(LifetimeManagerFactoryDelegate factory)
            {
                var child = Container.CreateChildContainer()
                                     .RegisterType(typeof(SingletonService), factory());

                var weak = new WeakReference(child);
                var instance = child.Resolve<SingletonService>();

                child.Dispose();

                return (weak, instance);
            }
        }

#if !BEHAVIOR_V4
        [PatternTestMethod(DISPOSE_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_CHILD)]
        [DynamicData(nameof(Disposable_Managers_Data))]
        public void Child_Child_Child_child_discarded(string name, LifetimeManagerFactoryDelegate factory, params Action<SingletonService>[] asserts)
        {
            var (weakChild, instance) = CreateResolveDiscard(factory);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weakChild.IsAlive);
            foreach (var assert in asserts) assert(instance);

            (WeakReference, SingletonService) CreateResolveDiscard(LifetimeManagerFactoryDelegate factory)
            {
                var child = Container.CreateChildContainer()
                                     .RegisterType(typeof(SingletonService), factory());

                var weak = new WeakReference(child);
                var instance = child.Resolve<SingletonService>();

                return (weak, instance);
            }
        }
#endif

        [PatternTestMethod(DISPOSE_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_CHILD)]
        [DynamicData(nameof(Disposable_Managers_Data))]
        public void Child_Child_Root_child_disposed(string name, LifetimeManagerFactoryDelegate factory, params Action<SingletonService>[] asserts)
        {
            var weak = new WeakReference(Container);
            var (weakChild, instance) = CreateResolveDispose(factory);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weak.IsAlive);
            Assert.IsFalse(weakChild.IsAlive);
            foreach (var assert in asserts) assert(instance);

            (WeakReference, SingletonService) CreateResolveDispose(LifetimeManagerFactoryDelegate factory)
            {
                var child = Container.CreateChildContainer()
                                     .RegisterType(typeof(SingletonService), factory());

                var weak = new WeakReference(child);
                var instance = child.Resolve<SingletonService>();

                Container.Dispose();
                Container = new UnityContainer(); // Must replace so GC collects

                return (weak, instance);
            }
        }

#if !BEHAVIOR_V4
        [PatternTestMethod(DISPOSE_NAME_FORMAT), TestProperty(RESOLVING, REGISTRATION_CHILD)]
        [DynamicData(nameof(Disposable_Managers_Data))]
        public void Child_Child_Root_child_discarded(string name, LifetimeManagerFactoryDelegate factory, params Action<SingletonService>[] asserts)
        {
            var weak = new WeakReference(Container);
            var (weakChild, instance) = CreateResolveDiscard(factory);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weak.IsAlive);
            Assert.IsFalse(weakChild.IsAlive);
            foreach (var assert in asserts) assert(instance);

            (WeakReference, SingletonService) CreateResolveDiscard(LifetimeManagerFactoryDelegate factory)
            {
                var child = Container.CreateChildContainer()
                                     .RegisterType(typeof(SingletonService), factory());

                var weak = new WeakReference(child);
                var instance = child.Resolve<SingletonService>();

                Container = new UnityContainer(); // Must replace so GC collects

                return (weak, instance);
            }
        }
#endif

        #endregion


        #region Implementation

        (WeakReference, SingletonService) CreateResolveAndDispose(LifetimeManagerFactoryDelegate factory)
        {
            var child = Container.CreateChildContainer();
            var weak = new WeakReference(child);
            var instance = child.Resolve<SingletonService>();

            Container.Dispose();
            Container = new UnityContainer(); // Must replace so GC collects

            return (weak, instance);
        }

        static void Disposed_True(SingletonService singleton) => Assert.IsTrue(singleton.IsDisposed, $"{nameof(singleton)} should be disposed");
        static void Disposed_False(SingletonService singleton) => Assert.IsFalse(singleton.IsDisposed, $"{nameof(singleton)} should not be disposed");

        #endregion


        #region Test Data

        public static IEnumerable<object[]> Disposable_Managers_Data
        {
            get
            {
                #region ContainerControlledLifetimeManager

                yield return new object[]
                {
                        typeof(ContainerControlledLifetimeManager).Name,
                        (LifetimeManagerFactoryDelegate)(() => new ContainerControlledLifetimeManager()),
                        (Action<SingletonService>)Disposed_True
                };

                #endregion

                #region ContainerControlledTransientManager
#if !UNITY_V4
                yield return new object[]
                {
                        typeof(ContainerControlledTransientManager).Name,
                        (LifetimeManagerFactoryDelegate)(() => new ContainerControlledTransientManager()),
                        (Action<SingletonService>)Disposed_True
                };
#endif
                #endregion

                #region ExternallyControlledLifetimeManager

                yield return new object[]
                {
                        typeof(ExternallyControlledLifetimeManager).Name,
                        (LifetimeManagerFactoryDelegate)(() => new ExternallyControlledLifetimeManager()),
                        (Action<SingletonService>)Disposed_False
                };

                #endregion

                #region HierarchicalLifetimeManager

                yield return new object[]
                {
                        typeof(HierarchicalLifetimeManager).Name,
                        (LifetimeManagerFactoryDelegate)(() => new HierarchicalLifetimeManager()),
                        (Action<SingletonService>)Disposed_True
                };

                #endregion

                #region PerThreadLifetimeManager

                yield return new object[]
                {
                        typeof(PerThreadLifetimeManager).Name,
                        (LifetimeManagerFactoryDelegate)(() => new PerThreadLifetimeManager()),
                        (Action<SingletonService>)Disposed_False
                };

                #endregion

                #region TransientLifetimeManager

                yield return new object[]
                {
                        typeof(TransientLifetimeManager).Name,
                        (LifetimeManagerFactoryDelegate)(() => new TransientLifetimeManager()),
                        (Action<SingletonService>)Disposed_False
                };

                #endregion
            }
        }

        #endregion
    }
}
