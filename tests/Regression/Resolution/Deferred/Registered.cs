using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Resolution
{
    public partial class Deferred
    {
        [TestMethod("Registered Deferred"), TestProperty(RESOLVING, REGISTERED)]
        public void Func_Registered()
        {
            // Act
            var resolver = Container.Resolve<Func<IService>>();

            // Verify
            Assert.IsNotNull(resolver);
            Assert.IsInstanceOfType(resolver, typeof(Func<IService>));
        }

        [TestMethod("Deferred resolves via container"), TestProperty(RESOLVING, REGISTERED)]
        public void Func_ResolvesThroughContainer()
        {
            // Act
            var resolver = Container.Resolve<Func<IService>>();

            // Verify
            var logger = resolver();

            Assert.IsInstanceOfType(resolver, typeof(Func<IService>));
            Assert.IsInstanceOfType(logger, typeof(Service));
        }

        [TestMethod("Deferred imported as dependency"), TestProperty(RESOLVING, REGISTERED)]
        public void Func_GetsInjectedAsADependency()
        {
            // Setup

            // Act
            var result = Container.Resolve<ObjectThatGetsAResolver>();

            // Verify
            Assert.IsNotNull(result.LoggerResolver);
            Assert.IsInstanceOfType(result, typeof(ObjectThatGetsAResolver));
            Assert.IsInstanceOfType(result.LoggerResolver(), typeof(Service));
        }

        [TestMethod("Deferred with matching name"), TestProperty(RESOLVING, REGISTERED)]
        public void Func_WithMatchingName()
        {
            // Act
            var resolver = Container.Resolve<Func<IService>>("1");

            // Verify
            Assert.IsNotNull(resolver);
            Assert.IsInstanceOfType(resolver, typeof(Func<IService>));
            Assert.IsInstanceOfType(resolver(), typeof(Service));
        }

        [TestMethod("Deferred with other name"), TestProperty(RESOLVING, REGISTERED)]
        public void Func_WithOtherName()
        {
            // Act
            var resolver = Container.Resolve<Func<IService>>("3");

            // Verify
            Assert.IsNotNull(resolver);
            Assert.IsInstanceOfType(resolver, typeof(Func<IService>));
            Assert.IsInstanceOfType(resolver(), typeof(OtherService));
        }

        [TestMethod("Deferred with not matching name"), TestProperty(RESOLVING, REGISTERED)]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void Func_WithNotMatchingName()
        {
            // Act
            var resolver = Container.Resolve<Func<IService>>("10");

            // Verify
            Assert.IsNotNull(resolver);
            Assert.IsInstanceOfType(resolver, typeof(Func<IService>));

            // This must throw
            var instance = resolver();
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(IService));

            Assert.Fail($"Failed to throw and the instance is not null: {null != instance}");
        }

        [TestMethod("Deferred enumerable"), TestProperty(RESOLVING, REGISTERED)]
        public void Func_OfIEnumerableCallsResolveAll()
        {
            // Setup
            Container.RegisterInstance("one", "first")
                     .RegisterInstance("two", "second")
                     .RegisterInstance("three", "third");

            // Act
            var resolver = Container.Resolve<Func<IEnumerable<string>>>();

            // Verify
            Assert.IsInstanceOfType(resolver, typeof(Func<IEnumerable<string>>));
            AreEquivalent(new string[] { "first", "second", "third" }, resolver().ToArray() );
        }
    }
}
