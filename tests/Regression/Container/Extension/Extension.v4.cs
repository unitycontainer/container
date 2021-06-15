using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression.Container;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity;
#elif UNITY_V5
using Unity.Extension;
using Unity.Builder;
using Unity;
#else
using Unity.Extension;
using Unity;
#endif

namespace Container
{
    public partial class Extensions
    {

        [TestMethod, TestProperty(TESTING, LEGACY)]
        public void ContainerCallsExtensionsInitializeMethod()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer();
            container.AddExtension(extension);

            Assert.IsTrue(extension.InitializeWasCalled);
        }

        [TestMethod, TestProperty(TESTING, LEGACY)]
        public void ExtensionReceivesExtensionContextInInitialize()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer();
            container.AddExtension(extension);

            Assert.IsNotNull(extension.Context);
            Assert.AreSame(container, extension.Context.Container);
        }

        [TestMethod, TestProperty(TESTING, LEGACY)]
        public void CanGetConfigurationInterfaceFromExtension()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            IMockConfiguration config = container.Configure<IMockConfiguration>();

            Assert.AreSame(extension, config);
            Assert.AreSame(container, config.Container);
        }

        [TestMethod, TestProperty(TESTING, LEGACY)]
        public void CanGetConfigurationWithoutGenericMethod()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            IMockConfiguration config = (IMockConfiguration)container.Configure(typeof(IMockConfiguration));

            Assert.AreSame(extension, config);
            Assert.AreSame(container, config.Container);
        }

        [TestMethod, TestProperty(TESTING, LEGACY)]
        public void ExtensionCanAddStrategy()
        {
            SpyStrategy spy = new SpyStrategy();
            SpyExtension extension = new SpyExtension(spy, UnityBuildStage.PostInitialization);

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            object result = container.Resolve<object>();
            Assert.IsTrue(spy.BuildUpWasCalled);
            Assert.AreSame(result, spy.Existing);
        }

        [TestMethod, TestProperty(TESTING, LEGACY)]
        public void ExtensionCanAddPolicy()
        {
            SpyStrategy spy = new SpyStrategy();
            SpyPolicy spyPolicy = new SpyPolicy();

            SpyExtension extension =
                new SpyExtension(spy, UnityBuildStage.PostInitialization, spyPolicy, typeof(SpyPolicy));

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            container.Resolve<object>();

            Assert.IsTrue(spyPolicy.WasSpiedOn);
        }
#if UNITY_V4
        [TestMethod, TestProperty(TESTING, LEGACY)]
        public void CanReinstallDefaultBehavior()
        {
            // This behavior no longer supported
            IUnityContainer container = new UnityContainer()
                .RemoveAllExtensions()
                .AddExtension(new UnityDefaultBehaviorExtension())
                .AddExtension(new UnityDefaultStrategiesExtension());

            object result = container.Resolve<object>();
            Assert.IsNotNull(result);
        }
#endif

        [TestMethod, TestProperty(TESTING, LEGACY)]
        public void CanLookupExtensionByClassName()
        {
            MockContainerExtension extension = new MockContainerExtension();
            IUnityContainer container = new UnityContainer();
            container.AddExtension(extension);

            MockContainerExtension result = container.Configure<MockContainerExtension>();

            Assert.AreSame(extension, result);
        }

        [TestMethod, TestProperty(TESTING, LEGACY)]
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

        [TestMethod, TestProperty(TESTING, LEGACY)]
        public void ChildContainerCreatedEventGivesChildContainerToExtension()
        {
            var mockExtension = new MockContainerExtension();
            ExtensionContext childContext = null;

            var container = new UnityContainer()
                .AddExtension(mockExtension);

            mockExtension.Context.ChildContainerCreated += (sender, ev) =>
            {
#if UNITY_V4 || UNITY_V5
                childContext = ev.ChildContext;
#else
                childContext = ev;
#endif
            };

            var child = container.CreateChildContainer();
            Assert.AreSame(child, childContext.Container);
        }

        [TestMethod, TestProperty(TESTING, LEGACY)]
        public void CanAddExtensionWithNonDefaultConstructor()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<ContainerExtensionWithNonDefaultConstructor>();
            var extension = container.Configure(typeof(ContainerExtensionWithNonDefaultConstructor));
            Assert.IsNotNull(extension);
        }
    }
}
