using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity.Tests.v5.Container
{
    [TestClass]
    public class GlobalSingletonResolutionFixture
    {
        [TestMethod]
        public void Test_ResolveSignletonInRootContainer_THEN_InstanceIsCreatedInRootContainer()
        {
            var rootContainer = CreateContainerForTests();
            var rootContainerId = rootContainer.GetHashCode();
            var childContainer1 = rootContainer.CreateChildContainer();
            var childContainer2 = rootContainer.CreateChildContainer();

            var reference1 = rootContainer.Resolve<ISingletonService>();
            var reference2 = childContainer1.Resolve<ISingletonService>();
            var reference3 = childContainer2.Resolve<ISingletonService>();

            Assert.AreEqual(reference1, reference2, "reference1 and reference2 must be same");
            Assert.AreEqual(reference1, reference3, "reference1 and reference3 must be same");

            Assert.AreEqual(rootContainerId, reference1.ContainerId, "Instance by reference1 should be created in root container");
            Assert.AreEqual(rootContainerId, reference2.ContainerId, "Instance by reference2 should be created in root container");
            Assert.AreEqual(rootContainerId, reference3.ContainerId, "Instance by reference3 should be created in root container");
        }

        [TestMethod]
        public void Test_ResolveSingletonInChildContainer_THEN_InstanceIsCreatedInRootContainer()
        {
            var rootContainer = CreateContainerForTests();
            var rootContainerId = rootContainer.GetHashCode();
            var childContainer1 = rootContainer.CreateChildContainer();
            var childContainer2 = rootContainer.CreateChildContainer();

            var reference1 = childContainer1.Resolve<ISingletonService>();
            var reference2 = childContainer2.Resolve<ISingletonService>();
            var reference3 = rootContainer.Resolve<ISingletonService>();

            Assert.AreEqual(reference1, reference2, "reference1 and reference2 must be same");
            Assert.AreEqual(reference1, reference3, "reference1 and reference3 must be same");

            Assert.AreEqual(rootContainerId, reference1.ContainerId, "Instance by reference1 should be created in root container");
            Assert.AreEqual(rootContainerId, reference2.ContainerId, "Instance by reference2 should be created in root container");
            Assert.AreEqual(rootContainerId, reference3.ContainerId, "Instance by reference3 should be created in root container");
        }


        [TestMethod]
        public void Test_UseSignletonInstance_AsFactory_WHEN_ItIsResolvedInRootContainer_THEN_ItCreatesItemsInRootContainer()
        {
            var rootContainer = CreateContainerForTests();
            var rootContainerId = rootContainer.GetHashCode();
            var childContainer1 = rootContainer.CreateChildContainer();
            var childContainer2 = rootContainer.CreateChildContainer();

            var reference1 = rootContainer.Resolve<ISingletonService>();
            var reference2 = childContainer1.Resolve<ISingletonService>();
            var reference3 = childContainer2.Resolve<ISingletonService>();

            var itemsFrom1 = reference1.GetElements();
            var itemsFrom2 = reference1.GetElements();
            var itemsFrom3 = reference1.GetElements();

            Assert.IsTrue(itemsFrom1.All(i => i.ContainerId == rootContainerId), "Not all items from instance by reference1 are created in root container");
            Assert.IsTrue(itemsFrom2.All(i => i.ContainerId == rootContainerId), "Not all items from instance by reference2 are created in root container");
            Assert.IsTrue(itemsFrom3.All(i => i.ContainerId == rootContainerId), "Not all items from instance by reference3 are created in root container");
        }

        [TestMethod]
        public void Test_UseSignletonInstance_AsFactory_WHEN_ItIsResolvedInChildContainer_THEN_ItCreatesItemsInRootContainer()
        {
            var rootContainer = CreateContainerForTests();
            var rootContainerId = rootContainer.GetHashCode();
            var childContainer1 = rootContainer.CreateChildContainer();
            var childContainer2 = rootContainer.CreateChildContainer();

            var reference1 = childContainer1.Resolve<ISingletonService>();
            var reference2 = childContainer2.Resolve<ISingletonService>();
            var reference3 = rootContainer.Resolve<ISingletonService>();

            var itemsFrom1 = reference1.GetElements();
            var itemsFrom2 = reference1.GetElements();
            var itemsFrom3 = reference1.GetElements();

            Assert.IsTrue(itemsFrom1.All(i => i.ContainerId == rootContainerId), "Not all items from instance by reference1 are created in root container");
            Assert.IsTrue(itemsFrom2.All(i => i.ContainerId == rootContainerId), "Not all items from instance by reference2 are created in root container");
            Assert.IsTrue(itemsFrom3.All(i => i.ContainerId == rootContainerId), "Not all items from instance by reference3 are created in root container");
        }


        [TestMethod]
        public void Test_UseSignletonInstance_AsFactory_WHEN_ItIsResolvedInRootContainer_AND_ChildContainerIsDisposed_THEN_ItWorksFromRootContainer()
        {
            var rootContainer = CreateContainerForTests();
            rootContainer.Resolve<ISingletonService>();

            var childContainer = rootContainer.CreateChildContainer();

            var singleton = childContainer.Resolve<ISingletonService>();
            childContainer.Dispose();

            var items = singleton.GetElements();

            Assert.AreEqual(10, items.Count(), "Unexepcted items count");
        }

        [TestMethod]
        public void Test_UseSignletonInstance_AsFactory_WHEN_ItIsResolvedInChildContainer_AND_ChildContainerIsDisposed_THEN_ItWorksFromRootContainer()
        {
            var rootContainer = CreateContainerForTests();

            var childContainer = rootContainer.CreateChildContainer();

            var singleton = childContainer.Resolve<ISingletonService>();
            childContainer.Dispose();

            var items = singleton.GetElements();

            Assert.AreEqual(10, items.Count(), "Unexepcted items count");
        }


        [TestMethod]
        public void Test_SingletonDisposing_WHEN_ItIsResolvedInChildContainer_AND_ChildContainerIsDisposed_THEN_ItIs_NotDisposed()
        {
            var rootContainer = CreateContainerForTests();

            var childContainer = rootContainer.CreateChildContainer();
            var singleton = childContainer.Resolve<ISingletonService>();

            childContainer.Dispose();

            Assert.IsFalse(singleton.IsDisposed, "Signleton instance should not be disposed");
        }

        [TestMethod]
        public void Test_SingletonDisposing_WHEN_ItIsResolvedInChildContainer_AND_RootContainerIsDisposed_THEN_ItIs_Disposed()
        {
            var rootContainer = CreateContainerForTests();

            var childContainer = rootContainer.CreateChildContainer();
            var singleton = childContainer.Resolve<ISingletonService>();

            rootContainer.Dispose();

            Assert.IsTrue(singleton.IsDisposed, "Signleton instance should be disposed");
        }

        [TestMethod]
        public void Test_SingletonDisposing_WHEN_ItIsResolvedInRootContainer_AND_ChildContainerIsDisposed_THEN_ItIs_NotDisposed()
        {
            var rootContainer = CreateContainerForTests();
            rootContainer.Resolve<ISingletonService>();

            var childContainer = rootContainer.CreateChildContainer();
            var singleton = childContainer.Resolve<ISingletonService>();

            childContainer.Dispose();

            Assert.IsFalse(singleton.IsDisposed, "Signleton instance should not be disposed");
        }

        [TestMethod]
        public void Test_SingletonDisposing_WHEN_ItIsResolvedInRootContainer_AND_RootContainerIsDisposed_THEN_ItIs_Disposed()
        {
            var rootContainer = CreateContainerForTests();

            var childContainer = rootContainer.CreateChildContainer();
            var singleton = childContainer.Resolve<ISingletonService>();

            rootContainer.Dispose();

            Assert.IsTrue(singleton.IsDisposed, "Signleton instance should be disposed");
        }


        [TestMethod]
        public void Test_SingletonCreatedDependenciesDisposing_WHEN_ItIsResolvedInRootContainer_AND_ChildContainerIsDisposed_THEN_ItDependencies_NotDisposed()
        {
            var rootContainer = CreateContainerForTests();
            rootContainer.Resolve<ISingletonService>();

            var childContainer = rootContainer.CreateChildContainer();
            var singleton = childContainer.Resolve<ISingletonService>();
            var items = singleton.GetElements();

            childContainer.Dispose();

            Assert.IsFalse(items.Any(i => i.IsDisposed), "Items created by signleton should not be disposed");
        }

        [TestMethod]
        public void Test_SingletonCreatedDependenciesDisposing_WHEN_ItIsResolvedInChildContainer_AND_ChildContainerIsDisposed_THEN_ItDependencies_NotDisposed()
        {
            var rootContainer = CreateContainerForTests();

            var childContainer = rootContainer.CreateChildContainer();
            var singleton = childContainer.Resolve<ISingletonService>();
            var items = singleton.GetElements();

            childContainer.Dispose();

            Assert.IsFalse(items.Any(i => i.IsDisposed), "Items created by signleton should not be disposed");
        }


        private IUnityContainer CreateContainerForTests()
        {
            var target = new UnityContainer();

            // transient type registrations, e.g. new instance is created every time
            target.RegisterType(typeof(ITestElement), typeof(TestElement));
            target.RegisterType(typeof(ITestElementFactory), typeof(TestElementFactory));

            // singleton type registration, e.g. same instance every time, from any scope (child or parent containter)
            target.RegisterType(typeof(ISingletonService), typeof(SingletonService), TypeLifetime.Singleton);

            return target;
        }

        #region Helper test-types

        interface ITestElement : IDisposable
        {
            bool IsDisposed { get; }

            long ContainerId { get; }
        }

        class TestElement : ITestElement
        {
            public long ContainerId { get; }

            public bool IsDisposed { get; private set; }

            public TestElement(IUnityContainer container)
            {
                ContainerId = container.GetHashCode();
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        interface ITestElementFactory
        {
            ITestElement CreateElement();
        }

        interface ISingletonService : IDisposable
        {
            long ContainerId { get; }

            bool IsDisposed { get; }

            IEnumerable<ITestElement> GetElements();
        }

        class TestElementFactory : ITestElementFactory
        {
            private readonly IUnityContainer _container;

            public TestElementFactory(IUnityContainer container)
            {
                _container = container;
            }

            public ITestElement CreateElement()
            {
                return _container.Resolve<ITestElement>();
            }
        }

        class SingletonService : ISingletonService
        {
            private readonly ITestElementFactory _elementFactory;

            public long ContainerId { get; }

            public bool IsDisposed { get; private set; }

            public SingletonService(IUnityContainer container, ITestElementFactory elementFactory)
            {
                ContainerId = container.GetHashCode();
                _elementFactory = elementFactory;
            }

            public IEnumerable<ITestElement> GetElements()
            {
                for (int i = 0; i < 10; i++)
                    yield return _elementFactory.CreateElement();
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        #endregion
    }
}
