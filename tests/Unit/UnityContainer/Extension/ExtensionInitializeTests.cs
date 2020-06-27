using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Container.Tests;
using Unity.Extension;

namespace Container.Extending
{
    [TestClass]
    public class ExtensionInitializeTests
    {
        MockContainerExtension    mock;
        OtherContainerExtension   other;
        DerivedContainerExtension derived;


        [TestInitialize]
        public void TestInitialize()
        {
            mock    = new MockContainerExtension();
            other   = new OtherContainerExtension();
            derived = new DerivedContainerExtension();
        }

        [TestMethod]
        public void AddInstances()
        {
            // Act
            _ = new UnityContainer { mock, other, derived };

            // Validate
            Assert.IsTrue(mock.InitializeWasCalled);
            Assert.IsTrue(other.InitializeWasCalled);
            Assert.IsTrue(derived.InitializeWasCalled);
        }

        [TestMethod]
        public void AddTypes()
        {
            // Act
            var container = new UnityContainer
            {
                typeof(MockContainerExtension),
                typeof(OtherContainerExtension),
                typeof(DerivedContainerExtension)
            };

            // Validate
            // Act
            var types = container.OfType<Type>().ToArray();

            // Validate
            Assert.AreEqual(3, types.Length);
        }

        [TestMethod]
        public void AddMix()
        {
            // Act
            var container = new UnityContainer
            {
                new MockContainerExtension(),
                typeof(OtherContainerExtension),
                typeof(DerivedContainerExtension)
            };

            // Validate
            // Act
            var types = container.OfType<Type>().ToArray();

            // Validate
            Assert.AreEqual(3, types.Length);
        }

        [TestMethod]
        public void ForEachTest()
        {
            // Arrange
            var types = new List<object>();
            var container = new UnityContainer { mock, other, derived };

            // Act
            foreach (var extension in container) types.Add(extension);

            // Validate
            Assert.AreEqual(3, types.Count);
        }

        [TestMethod]
        public void LinqTest()
        {
            // Arrange
            var container = new UnityContainer { mock, other, derived };

            // Act
            var types = container.OfType<Type>().ToArray();

            // Validate
            Assert.AreEqual(3, types.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddWrongType()
        {
            // Act
            _ = new UnityContainer { typeof(ExtensionInitializeTests) };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullType()
        {
            // Act
            _ = new UnityContainer { (Type)null };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullExtension()
        {
            // Act
            _ = new UnityContainer { (UnityContainerExtension)null };
        }
    }
}
