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
        [DynamicData(nameof(Child_Scope_Data))]
        [PatternTestMethod("Child Container"), TestCategory(CHILD_SCOPE)]
        public virtual void FromChildContainer(string name,
                                               LifetimeManagerFactoryDelegate factory, Type type,
                                               Action<object, object> assert,
                                               Action<object, object> assertDifferentThreads)
        {
            // Arrange
            ArrangeTest(factory, type);

            // Act
            Item1 = Container.Resolve(TargetType);
            Item2 = Container.CreateChildContainer()
                             .Resolve(TargetType);

            // Validate
            Assert.IsNotNull(Item1);
            Assert.IsNotNull(Item2);

            Assert.IsInstanceOfType(Item1, TargetType);
            Assert.IsInstanceOfType(Item1, TargetType);

            assert(Item1, Item2);
        }

        [DynamicData(nameof(Child_Scope_Data))]
        [PatternTestMethod("Registered in Child and Root"), TestCategory(CHILD_SCOPE)]
        public virtual void RegisteredInChildAndRoot(string name,
                                                     LifetimeManagerFactoryDelegate factory, Type type,
                                                     Action<object, object> assert,
                                                     Action<object, object> assertDifferentThreads)
        {
            // Arrange
            var child = Container.CreateChildContainer();
            ArrangeTest(factory, type, child);

            // Act
            Item1 = Container.Resolve(TargetType);
            Item2 = child.Resolve(TargetType);

            // Validate
            Assert.IsNotNull(Item1);
            Assert.IsNotNull(Item2);

            Assert.IsInstanceOfType(Item1, TargetType);
            Assert.IsInstanceOfType(Item1, TargetType);

            Assert.AreNotSame(Item1, Item2);
        }


        [DynamicData(nameof(Child_Scope_Data))]
        [PatternTestMethod("Child Container as import"), TestCategory(CHILD_SCOPE)]
        public virtual void FromChildContainer_Import(string name,
                                                      LifetimeManagerFactoryDelegate factory, Type type,
                                                      Action<object, object> assert,
                                                      Action<object, object> assertDifferentThreads)
        {
            // Exclusion
            if (ArrangeTest(factory, type)) return;

            // Act
            Item1 = Container.Resolve<Foo<Service>>().Value;
            Item2 = Container.CreateChildContainer()
                             .Resolve<Foo<Service>>().Value;

            // Validate
            Assert.IsNotNull(Item1);
            Assert.IsNotNull(Item2);

            assert(Item1, Item2);
        }


#if !BEHAVIOR_V4 // Unity v4 did not support multi-threading
        [DynamicData(nameof(Child_Scope_Data))]
        [PatternTestMethod("Child Container on different threads"), TestCategory(CHILD_SCOPE)]
        public virtual void FromChildContainer_DifferentThreads(string name,
                                                                LifetimeManagerFactoryDelegate factory, Type type,
                                                                Action<object, object> assert,
                                                                Action<object, object> assertDifferentThreads)
        {
            // Arrange
            ArrangeTest(factory, type);

            // Act
            Thread t1 = new Thread(new ParameterizedThreadStart((c) => Item1 = Container.Resolve(TargetType)));
            Thread t2 = new Thread(new ParameterizedThreadStart((c) => Item2 = Container.CreateChildContainer()
                                                                                        .Resolve(TargetType)));

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
