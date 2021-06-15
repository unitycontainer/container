using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [PatternTestMethod("Built-In Interface"), TestProperty(RESOLVING, nameof(FromEmpty))]
        [DynamicData(nameof(BuiltInTypes_Data), typeof(PatternBase))]
        public virtual void BuiltIn_Interface(string test, Type type)
        {
            // Act
            var instance = Container.Resolve(type, null);

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, type);
            Assert.AreSame(Container, instance);
        }

        [PatternTestMethod("Built-In Interface in child"), TestProperty(RESOLVING, nameof(FromEmpty))]
        [DynamicData(nameof(BuiltInTypes_Data), typeof(PatternBase))]
        public virtual void BuiltIn_InChild(string test, Type type)
        {
            // Arrange
            var child = Container.CreateChildContainer();

            // Act
            var instance = child.Resolve(type, null);

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, type);
            Assert.AreSame(child, instance);
        }

        [PatternTestMethod("Built-In Interface named"), TestProperty(RESOLVING, nameof(FromEmpty))]
        [DynamicData(nameof(BuiltInTypes_Data), typeof(PatternBase))]
        [ExpectedException(typeof(ResolutionFailedException))]
        public virtual void BuiltIn_Named(string test, Type type)
        {
            // Act
            _ = Container.Resolve(type, Name);
        }

#if !UNITY_V4
        [TestMethod("Enumerable<..> from different threads"), TestProperty(RESOLVING, nameof(FromEmpty))]
        public void Enumerable_Multithreaded()
        {
            var barrier = new Barrier(2);
            var storage = new IEnumerable<Service>[2];

            // Act
            Parallel.Invoke(MaxDegreeOfParallelism,
                () => {
                    barrier.SignalAndWait();
                    storage[0] = Container.Resolve<IEnumerable<Service>>();
                },
                () => {
                    barrier.SignalAndWait();
                    storage[1] = Container.Resolve<IEnumerable<Service>>();
                });

            // Validate
            Assert.IsFalse(Container.IsRegistered<Service>());

            Assert.IsNotNull(storage[0]);
            Assert.IsNotNull(storage[1]);

            // All different instances
            Assert.AreNotSame(storage[0], storage[1]);

            // All different threads
            Assert.AreEqual(1, storage[0].Count());
            Assert.AreEqual(1, storage[1].Count());
        }
#endif
    }
}
