using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Extension;
using Unity.Tests.TestDoubles;

namespace Unity.Tests.Container
{
    [TestClass]
    public class UnityExtensionFixture
    {
        [TestMethod]
        public void ContainerCallsExtensionsInitializeMethod()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer();
            container.AddExtension(extension);

            Assert.IsTrue(extension.InitializeWasCalled);
        }

        [TestMethod]
        public void ExtensionReceivesExtensionContextInInitialize()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer();
            container.AddExtension(extension);

            Assert.IsNotNull(extension.Context);
            Assert.AreSame(container, extension.Context.Container);
        }

        [TestMethod]
        public void CanGetConfigurationInterfaceFromExtension()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            IMockConfiguration config = container.Configure<IMockConfiguration>();

            Assert.AreSame(extension, config);
            Assert.AreSame(container, config.Container);
        }

        [TestMethod]
        public void CanGetConfigurationWithoutGenericMethod()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            IMockConfiguration config = (IMockConfiguration)container.Configure(typeof(IMockConfiguration));

            Assert.AreSame(extension, config);
            Assert.AreSame(container, config.Container);
        }

        [TestMethod]
        public void ExtensionCanAddStrategy()
        {
            var spy = new SpyStrategy();
            SpyExtension extension = new SpyExtension(spy, Stage.PreCreation);

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            object result = container.Resolve<object>();
            Assert.IsTrue(spy.BuildUpWasCalled);
            Assert.AreSame(result, spy.Existing);
        }

        [TestMethod]
        public void ExtensionCanAddPolicy()
        {
            var spy = new SpyStrategy();
            SpyPolicy spyPolicy = new SpyPolicy();

            SpyExtension extension =
                new SpyExtension(spy, Stage.PreCreation, spyPolicy, typeof(SpyPolicy));

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            container.Resolve<object>();

            Assert.IsTrue(spyPolicy.WasSpiedOn);
        }

        [TestMethod]
        public void CanLookupExtensionByClassName()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer();
            container.AddExtension(extension);

            MockContainerExtension result = container.Configure<MockContainerExtension>();

            Assert.AreSame(extension, result);
        }

        [TestMethod]
        public void ContainerRaisesChildContainerCreatedToExtension()
        {
            bool childContainerEventRaised = false;
            var mockExtension = new MockContainerExtension();

            var container = new UnityContainer()
                .AddExtension(mockExtension);

            mockExtension.Context.ChildContainerCreated += (sender, ev) =>
                {
                    childContainerEventRaised = true;
                };

            var child = container.CreateChildContainer();
            Assert.IsTrue(childContainerEventRaised);
        }

        [TestMethod]
        public void ChildContainerCreatedEventGivesChildContainerToExtension()
        {
            var mockExtension = new MockContainerExtension();
            ExtensionContext childContext = null;

            var container = new UnityContainer()
                .AddExtension(mockExtension);

            mockExtension.Context.ChildContainerCreated += (sender, ev) =>
            {
                childContext = ev.ChildContext;
            };

            var child = container.CreateChildContainer();
            Assert.AreSame(child, childContext.Container);
        }

        [TestMethod]
        public void CanAddExtensionWithNonDefaultConstructor()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<ContainerExtensionWithNonDefaultConstructor>();
            var extension = container.Configure(typeof(ContainerExtensionWithNonDefaultConstructor));
            Assert.IsNotNull(extension);
        }
    }
}
