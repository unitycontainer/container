using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Threading;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Lifetime.Manager
{
    public abstract partial class Pattern
    {
        [DynamicData(nameof(Same_Scope_Data))]
        [PatternTestMethod("Same Container"), TestCategory(SAME_SCOPE)]
        public virtual void FromSameContainer(string name, 
                                              LifetimeManagerFactoryDelegate factory, Type type,
                                              Action<object, object> assertScope,
                                              Action<object, object> assertDifferentThreads)
        {
            // Arrange
            ArrangeTest(factory, type);

            // Act
            Item1 = Container.Resolve(TargetType);
            Item2 = Container.Resolve(TargetType);

            // Validate
            Assert.IsNotNull(Item1);
            Assert.IsNotNull(Item2);

            Assert.IsInstanceOfType(Item1, TargetType);
            Assert.IsInstanceOfType(Item1, TargetType);

            assertScope(Item1, Item2);
        }


        [DynamicData(nameof(Same_Scope_Data))]
        [PatternTestMethod("Same child Container"), TestCategory(SAME_SCOPE)]
        public virtual void SameChildContainer(string name,
                                               LifetimeManagerFactoryDelegate factory, Type type,
                                               Action<object, object> assertScope,
                                               Action<object, object> assertDifferentThreads)
        {
            // Arrange
            ArrangeTest(factory, type);
            var child = Container.CreateChildContainer();

            // Act
            Item1 = child.Resolve(TargetType);
            Item2 = child.Resolve(TargetType);

            // Validate
            Assert.IsNotNull(Item1);
            Assert.IsNotNull(Item2);

            Assert.IsInstanceOfType(Item1, TargetType);
            Assert.IsInstanceOfType(Item1, TargetType);

            assertScope(Item1, Item2);
        }


        [DynamicData(nameof(Same_Scope_Data))]
        [PatternTestMethod("Same Container as import"), TestCategory(SAME_SCOPE)]
        public virtual void FromSameContainer_Import(string name,
                                                     LifetimeManagerFactoryDelegate factory, Type type,
                                                     Action<object, object> assertScope,
                                                     Action<object, object> assertDifferentThreads)
        {
            // Exclusion
            if (ArrangeTest(factory, type)) return;

            // Act
            Item1 = Container.Resolve<Foo<Service>>().Value;
            Item2 = Container.Resolve<Foo<Service>>().Value;

            // Validate
            Assert.IsNotNull(Item1);
            Assert.IsNotNull(Item2);

            assertScope(Item1, Item2);
        }


        [DynamicData(nameof(Same_Scope_Data))]
        [PatternTestMethod("Same child Container as import"), TestCategory(SAME_SCOPE)]
        public virtual void SameChildContainer_Import(string name,
                                                      LifetimeManagerFactoryDelegate factory, Type type,
                                                      Action<object, object> assertScope,
                                                      Action<object, object> assertThreads)
        {
            // Exclusion
            if (ArrangeTest(factory, type)) return;
            var child = Container.CreateChildContainer();

            // Act
            Item1 = child.Resolve<Foo<Service>>().Value;
            Item2 = child.Resolve<Foo<Service>>().Value;

            // Validate
            Assert.IsNotNull(Item1);
            Assert.IsNotNull(Item2);

            assertScope(Item1, Item2);
        }


#if !BEHAVIOR_V4 // Unity v4 did not support multi-threading
        [DynamicData(nameof(Same_Scope_Data))]
        [PatternTestMethod("Same container on different threads"), TestCategory(SAME_SCOPE)]
        public virtual void FromSameContainer_DifferentThreads(string name,
                                                               LifetimeManagerFactoryDelegate factory, Type type,
                                                               Action<object, object> assertScope,
                                                               Action<object, object> assertDifferentThreads)
        {
            // Arrange
            ArrangeTest(factory, type);

            // Act
            Thread t1 = new Thread(new ParameterizedThreadStart((c) => Item1 = Container.Resolve(TargetType)));
            Thread t2 = new Thread(new ParameterizedThreadStart((c) => Item2 = Container.Resolve(TargetType)));

            t1.Start("1");
            t2.Start("2");
            t1.Join();
            t2.Join();

            Assert.IsInstanceOfType(Item1, TargetType);
            Assert.IsInstanceOfType(Item1, TargetType);

            assertDifferentThreads(Item1, Item2);
        }

        [DynamicData(nameof(Same_Scope_Data))]
        [PatternTestMethod("Same child container on different threads"), TestCategory(SAME_SCOPE)]
        public virtual void SameChildContainer_DifferentThreads(string name,
                                                                LifetimeManagerFactoryDelegate factory, Type type,
                                                                Action<object, object> assertScope,
                                                                Action<object, object> assertDifferentThreads)
        {
            // Arrange
            ArrangeTest(factory, type);
            var child = Container.CreateChildContainer();

            // Act
            Thread t1 = new Thread(new ParameterizedThreadStart((c) => Item1 = child.Resolve(TargetType)));
            Thread t2 = new Thread(new ParameterizedThreadStart((c) => Item2 = child.Resolve(TargetType)));

            t1.Start("1");
            t2.Start("2");
            t1.Join();
            t2.Join();

            Assert.IsInstanceOfType(Item1, TargetType);
            Assert.IsInstanceOfType(Item1, TargetType);

            assertDifferentThreads(Item1, Item2);
        }
#endif
    }
}
