// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests
{
    [TestClass]
    public class ContainerDefaultContentFixture
    {
        [TestMethod]
        [Ignore]
        public void WhenResolvingAnIUnityContainerItResolvesItself()
        {
            IUnityContainer container = new UnityContainer();

            IUnityContainer resolvedContainer = container.Resolve<IUnityContainer>();

            Assert.AreSame(container, resolvedContainer);
        }

        [TestMethod]
        [Ignore]
        public void WhenResolveingAnIUnityContainerForAChildContainerItResolvesTheChildContainer()
        {
            IUnityContainer container = new UnityContainer();
            IUnityContainer childContainer = container.CreateChildContainer();

            IUnityContainer resolvedContainer = childContainer.Resolve<IUnityContainer>();

            Assert.AreSame(childContainer, resolvedContainer);
        }

        [TestMethod]
        [Ignore]
        public void AClassThatHasADependencyOnTheContainerGetsItInjected()
        {
            IUnityContainer container = new UnityContainer();

            IUnityContainerInjectionClass obj;
            obj = container.Resolve<IUnityContainerInjectionClass>();

            Assert.AreSame(container, obj.Container);
        }

        public class IUnityContainerInjectionClass
        {
            [Dependency]
            public IUnityContainer Container { get; set; }
        }
    }
}