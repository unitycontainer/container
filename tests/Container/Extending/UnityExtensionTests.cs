using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;

namespace Container.Extending
{
    [TestClass]
    public class UnityExtensionTests
    {
        UnityContainer            container;
        MockContainerExtension    mock;
        OtherContainerExtension   other;
        DerivedContainerExtension derived;

        [TestInitialize]
        public void TestInitialize()
        {
            container = new UnityContainer();
            mock      = new MockContainerExtension();
            other     = new OtherContainerExtension();
            derived   = new DerivedContainerExtension();
        }

        [TestMethod]
        public void AddExtensionTest()
        {
            // Act
            container.AddExtension(mock)
                     .AddExtension(other)
                     .AddExtension(derived);

            // Validate
            Assert.IsTrue(mock.InitializeWasCalled);
            Assert.IsTrue(other.InitializeWasCalled);
            Assert.IsTrue(derived.InitializeWasCalled);
        }

        [TestMethod]
        public void ConfigureTest()
        {
            // Act
            container.AddExtension(mock)
                     .AddExtension(other)
                     .AddExtension(derived);

            // Validate
            Assert.AreSame(derived, container.Configure(typeof(DerivedContainerExtension)));
            Assert.IsNull(container.Configure(typeof(OtherContainerExtension)));
            Assert.AreSame(mock, container.Configure(typeof(MockContainerExtension)));
        }

        [TestMethod]
        public void ConfigureGenericTest()
        {
            // Act
            container.AddExtension(mock)
                     .AddExtension(other)
                     .AddExtension(derived);

            // Validate
            Assert.AreSame(derived, container.Configure<DerivedContainerExtension>());
            Assert.AreSame(mock,    container.Configure<MockContainerExtension>());
        }

        [TestMethod]
        public void AddExtensionGenericTest()
        {
            // Act
            container.AddNewExtension<MockContainerExtension>()
                     .AddNewExtension<OtherContainerExtension>()
                     .AddNewExtension<DerivedContainerExtension>();

            // Validate
            Assert.IsTrue(container.Configure<MockContainerExtension>().InitializeWasCalled);
            Assert.IsTrue(container.Configure<DerivedContainerExtension>().InitializeWasCalled);
        }

        [TestMethod]
        public void ContainerAddExtension()
        {
            // Act
            container.AddExtension<MockContainerExtension>()
                     .AddExtension<OtherContainerExtension>()
                     .AddExtension<DerivedContainerExtension>();

            // Validate
            Assert.IsTrue(container.Configure<MockContainerExtension>().InitializeWasCalled);
            Assert.IsTrue(container.Configure<DerivedContainerExtension>().InitializeWasCalled);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainerNullAddNew()
        {
            UnityContainer unity = null;

            unity.AddNewExtension<MockContainerExtension>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainerNullAddExtension()
        {
            UnityContainer unity = null;

            unity.AddExtension<MockContainerExtension>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainerNullAdd()
        {
            UnityContainer unity = null;

            unity.Add(new MockContainerExtension());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainerNullAddType()
        {
            UnityContainer unity = null;

            unity.Add(typeof(MockContainerExtension));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainerNullConfigure()
        {
            UnityContainer unity = null;

            unity.Configure<MockContainerExtension>();
        }
    }
}
