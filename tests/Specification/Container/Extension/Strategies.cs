using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression.Container;
using System.Collections;
using System.Linq;
using System;
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
        [TestMethod("Can get Extension Context"), TestProperty(TESTING, nameof(BuilderStrategy))]
        public void CanGetExtensionContext()
        {
            SpyStrategy spy = new SpyStrategy();

            var extension = new UnityContainer()
                .AddExtension(new SpyExtension(spy, UnityBuildStage.PreCreation))
                .Configure<SpyExtension>();

            Assert.IsNotNull(extension);
            Assert.IsNotNull(extension.Container);
            Assert.IsNotNull(extension.ExtensionContext);
        }

        [TestMethod("Can Enumerate Strategies"), TestProperty(TESTING, nameof(BuilderStrategy))]
        [Obsolete]
        public void CanEnumerateStrategies()
        {
            SpyStrategy spy = new SpyStrategy();

            var extension = new UnityContainer()
                .AddExtension(new SpyExtension(spy, UnityBuildStage.PreCreation))
                .Configure<SpyExtension>();

            var enumerable = AsEnumerable(extension.ExtensionContext.Strategies);

            Assert.IsTrue(enumerable is IEnumerable);
        }

        [TestMethod("Can Add Strategy"), TestProperty(TESTING, nameof(BuilderStrategy))]
        [Obsolete]
        public void CanAddStrategy()
        {
            SpyStrategy spy = new SpyStrategy();

            var extension = new UnityContainer()
                .AddExtension(new SpyExtension(spy, UnityBuildStage.PreCreation))
                .Configure<SpyExtension>();

            var strategies = AsEnumerable(extension.ExtensionContext.Strategies)
                .Cast<BuilderStrategy>()
                .ToArray();

            Assert.IsTrue(strategies.Contains(spy));
        }

#if !UNITY_V4
        [TestMethod("Can Add Strategy Twice"), TestProperty(TESTING, nameof(BuilderStrategy))]
        [Obsolete]
        [ExpectedException(typeof(ArgumentException))]
        public void CanAddStrategyTwice()
        {
            SpyStrategy spy = new SpyStrategy();

            var extension = new UnityContainer()
                .AddExtension(new SpyExtension(spy, UnityBuildStage.PreCreation))
                .Configure<SpyExtension>();

            var before = AsEnumerable(extension.ExtensionContext.Strategies)
                .Cast<BuilderStrategy>()
                .ToArray();

            Assert.IsTrue(before.Contains(spy));

            extension.ExtensionContext.Strategies.Add(spy, UnityBuildStage.PreCreation);
            
            var after = AsEnumerable(extension.ExtensionContext.Strategies)
                .Cast<BuilderStrategy>()
                .ToArray();

            Assert.AreNotEqual(before.Length, after.Length);
        }
#endif

#if !UNITY_V4 && !UNITY_V5 && !UNITY_V6

        [TestMethod("Can Add Other Strategy"), TestProperty(TESTING, nameof(BuilderStrategy))]
        public void CanAddOtherStrategy()
        {
            SpyStrategy spy = new SpyStrategy();

            var extension = new UnityContainer()
                .AddExtension(new SpyExtension(spy, UnityBuildStage.PreCreation))
                .Configure<SpyExtension>();

            var before = AsEnumerable(extension.ExtensionContext.TypePipelineChain)
                .Cast<BuilderStrategy>()
                .ToArray();

            Assert.IsTrue(before.Contains(spy));

            extension.ExtensionContext.TypePipelineChain.Add(spy, UnityBuildStage.PostCreation);

            var after = AsEnumerable(extension.ExtensionContext.TypePipelineChain)
                .Cast<BuilderStrategy>()
                .ToArray();

            Assert.AreNotEqual(before.Length, after.Length);
        }

        [TestMethod("Can Replace Strategy"), TestProperty(TESTING, nameof(BuilderStrategy))]
        public void CanReplaceStrategy()
        {
            SpyStrategy spy = new SpyStrategy();

            var extension = new UnityContainer()
                .AddExtension(new SpyExtension(spy, UnityBuildStage.PreCreation))
                .Configure<SpyExtension>();

            var before = AsEnumerable(extension.ExtensionContext.TypePipelineChain)
                .Cast<BuilderStrategy>()
                .ToArray();

            Assert.IsTrue(before.Contains(spy));

            extension.ExtensionContext.TypePipelineChain[UnityBuildStage.PreCreation] = spy;

            var after = AsEnumerable(extension.ExtensionContext.TypePipelineChain)
                .Cast<BuilderStrategy>()
                .ToArray();

            Assert.AreEqual(before.Length, after.Length);
        }
#endif

    }
}
