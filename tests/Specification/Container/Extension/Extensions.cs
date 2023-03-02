using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression.Container;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
#elif UNITY_V5 || UNITY_V6
using Unity.Strategies;
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
        [PatternTestMethod(NAME_PATTERN), TestProperty(TESTING, nameof(BuilderStrategy))]
        public void ExtensionCanAddStrategy_PreCreation()
        {
            SpyStrategy spy = new SpyStrategy();
            SpyExtension extension = new SpyExtension(spy, UnityBuildStage.PreCreation);

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            object result = container.Resolve<object>();
            Assert.IsTrue(spy.BuildUpWasCalled);
            Assert.AreSame(result, spy.Existing);
        }

        [PatternTestMethod(NAME_PATTERN), TestProperty(TESTING, nameof(BuilderStrategy))]
        public void ExtensionCanAddStrategy_PostInitialization()
        {
            SpyStrategy spy = new SpyStrategy();
            SpyExtension extension = new SpyExtension(spy, UnityBuildStage.PostInitialization);

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            object result = container.Resolve<object>();
            Assert.IsTrue(spy.BuildUpWasCalled);
            Assert.AreSame(result, spy.Existing);
        }


        [TestMethod("Can Add Default Policy"), TestProperty(TESTING, POLICY)]
        public void ExtensionCanAddDefaultPolicy()
        {
            SpyStrategy spy = new SpyStrategy();
            SpyPolicy spyPolicy = new SpyPolicy();

            SpyExtension extension =
                new SpyExtension(spy, UnityBuildStage.PreCreation, spyPolicy, typeof(SpyPolicy));

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            container.Resolve<object>();

            Assert.IsTrue(spyPolicy.WasSpiedOn);
        }
    }
}
