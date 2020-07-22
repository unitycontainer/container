using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Extension;

namespace Container.Extending
{
    [TestClass]
    public class ExtensionInitializationTests
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
        public void Baseline()
        {
            // Act
            var types = new UnityContainer().ToArray();
            // Validate
            Assert.AreEqual(0, types.Length);
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
            var types = container.ToArray();

            // Validate
            Assert.AreEqual(2, types.Length);
        }


        [TestMethod]
        public void AddLambda()
        {
            ExtensionContext local = null;

            // Act
            var container = new UnityContainer
            {
                (context) => local = context 
            };

            // Validate
            // Act
            var types = container.ToArray();

            // Validate
            Assert.AreEqual(0, types.Length);
            Assert.IsNotNull(local);
            Assert.AreSame(container, local.Container);
        }

        [TestMethod]
        public void AddMix()
        {
            ExtensionContext local = null;

            // Act
            var container = new UnityContainer
            {
                new MockContainerExtension(),
                typeof(OtherContainerExtension),
                typeof(DerivedContainerExtension),
                (context) => local = context
            };

            // Validate
            // Act
            var types = container.ToArray();

            // Validate
            Assert.AreEqual(2, types.Length);
            Assert.IsNotNull(local);
            Assert.AreSame(container, local.Container);
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
            Assert.AreEqual(2, types.Count);
        }

        [TestMethod]
        public void LinqTest()
        {
            // Arrange
            var container = new UnityContainer { mock, other, derived };

            // Act
            var types = container.Cast<Type>().ToArray();

            // Validate
            Assert.AreEqual(2, types.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddWrongType()
        {
            // Act
            _ = new UnityContainer { typeof(ExtensionInitializationTests) };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullType()
        {
            // Act
            _ = new UnityContainer { (Type)null };
        }

        [TestMethod]
        public void AddNullExtension()
        {
            // Act
            _ = new UnityContainer { (UnityContainerExtension)null };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullAction()
        {
            // Act
            _ = new UnityContainer { (Action<ExtensionContext>)null };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddActionNull()
        {
            UnityContainer unity = null;
            // Act
            unity.Add((Action<ExtensionContext>)null);
        }
    }
}
