using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Resolution
{
    public partial class Lazy
    {
        [TestMethod]
        public void CanResolveALazy()
        {
            // Setup
            Container.RegisterType<IService, Service>();

            // Act
            var lazy = Container.Resolve<Lazy<IService>>();

            // Verify
            Assert.IsNotNull(lazy);
        }

        [TestMethod]
        public void ResolvedLazyHasNoValue()
        {
            // Setup
            Container.RegisterType<IService, Service>();

            // Act
            var lazy = Container.Resolve<Lazy<IService>>();

            // Verify
            Assert.IsFalse(lazy.IsValueCreated);
        }

        [TestMethod]
        public void ResolvedLazyResolvesThroughContainer()
        {
            // Setup
            Container.RegisterType<IService, Service>();

            // Act
            var lazy = Container.Resolve<Lazy<IService>>();
            var logger = lazy.Value;

            // Verify
            Assert.IsInstanceOfType(logger, typeof(Service));
        }

        [TestMethod]
        public void ResolvedLazyGetsInjectedAsADependency()
        {
            // Setup
            Container.RegisterType<IService, Service>();

            // Act
            var result = Container.Resolve<ObjectThatGetsALazy>();

            // Verify
            Assert.IsNotNull(result.LoggerLazy);
            Assert.IsInstanceOfType(result.LoggerLazy.Value, typeof(Service));
        }

        [TestMethod]
        public void CanResolveLazyWithName()
        {
            // Setup
            Container.RegisterType<IService, Service>()
                     .RegisterType<IService, OtherService>("special");

            // Act
            var lazy = Container.Resolve<Lazy<IService>>("special");

            // Verify
            Assert.IsNotNull(lazy);
        }

        [TestMethod]
        public void ResolvedLazyWithNameResolvedThroughContainerWithName()
        {
            // Setup
            Container
                .RegisterType<IService, Service>()
                .RegisterType<IService, OtherService>("special");

            // Act
            var lazy = Container.Resolve<Lazy<IService>>("special");
            var result = lazy.Value;

            // Verify
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OtherService));
        }

        [TestMethod]
        public void DifferentResolveCallsReturnDifferentLazyInstances()
        {
            // Setup
            Container
                .RegisterType<IService, Service>();

            // Act
            var lazy1 = Container.Resolve<Lazy<IService>>();
            var lazy2 = Container.Resolve<Lazy<IService>>();

            // Verify
            Assert.AreNotSame(lazy1.Value, lazy2.Value);
        }

        [TestMethod]
        public void DifferentLazyGenericsGetTheirOwnBuildPlan()
        {
            // Setup
            Container
                .RegisterType<IService, Service>()
                .RegisterInstance<string>("the instance");

            // Act
            var lazy1 = Container.Resolve<Lazy<IService>>();
            var lazy2 = Container.Resolve<Lazy<string>>();

            // Verify
            Assert.IsInstanceOfType(lazy1.Value, typeof(IService));
            Assert.AreEqual("the instance", lazy2.Value);
        }

        [TestMethod]
        public void ObservesPerResolveSingleton()
        {
            // Setup
            Container
                .RegisterType<IService, Service>()
                .RegisterType(typeof(Lazy<>), new PerResolveLifetimeManager());

            // Act
            var result = Container.Resolve<ObjectThatGetsMultipleLazy>();

            // Verify
            Assert.IsNotNull(result.LoggerLazy1);
            Assert.IsNotNull(result.LoggerLazy2);
            Assert.AreSame(result.LoggerLazy1, result.LoggerLazy2);
            Assert.IsInstanceOfType(result.LoggerLazy1.Value, typeof(Service));
            Assert.IsInstanceOfType(result.LoggerLazy2.Value, typeof(Service));
            Assert.AreSame(result.LoggerLazy1.Value, result.LoggerLazy2.Value);

            var value1 = result.LoggerLazy1.Value;
            var value2 = Container.Resolve<Lazy<IService>>().Value;

            Assert.AreNotSame(value1, value2);
        }

        [TestMethod]
        public void ResolvingLazyOfIEnumerableCallsResolveAll()
        {
            // Setup
            Container
                .RegisterInstance("one", "first")
                .RegisterInstance("two", "second")
                .RegisterInstance("three", "third");

            // Act
            var lazy = Container.Resolve<Lazy<IEnumerable<string>>>();
            var result = lazy.Value;

            // Verify
            result.SequenceEqual(new[] {"first", "second", "third"});
        }
    }
}
