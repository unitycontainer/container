using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        [TestMethod("Unregistered Deferred"), TestProperty(RESOLVING, UNREGISTERED)]
        public void Func_Unregistered()
        {
            // Act
            var resolver = Container.Resolve<Func<object>>();

            // Verify
            Assert.IsNotNull(resolver);
            Assert.IsInstanceOfType(resolver, typeof(Func<object>));

            var instance = resolver();
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(object));
        }

        [TestMethod("Deferred resolves via container"), TestProperty(RESOLVING, UNREGISTERED)]
        public void Func_UnregisteredThroughContainer()
        {
            // Act
            var resolver = Container.Resolve<Func<object>>();

            // Verify
            var logger = resolver();

            Assert.IsInstanceOfType(resolver, typeof(Func<object>));
            Assert.IsInstanceOfType(logger, typeof(object));
        }

        [TestMethod("Deferred with name"), TestProperty(RESOLVING, UNREGISTERED)]
        public void Func_UnregisteredWithMatchingName()
        {
            // Act
            var resolver = Container.Resolve<Func<object>>("1");

            // Verify
            Assert.IsNotNull(resolver);
            Assert.IsInstanceOfType(resolver, typeof(Func<object>));
            Assert.IsInstanceOfType(resolver(), typeof(object));
        }

        [TestMethod("Deferred with other name"), TestProperty(RESOLVING, UNREGISTERED)]
        public void Func_UnregisteredWithOtherName()
        {
            var resolver = Container.Resolve<Func<object>>("1");
            // Verify
            Assert.IsNotNull(resolver);
            Assert.IsInstanceOfType(resolver, typeof(Func<object>));
            var instance = resolver();
            Assert.IsInstanceOfType(instance, typeof(object));

            // Act
            resolver = Container.Resolve<Func<object>>("3");

            // Verify
            Assert.IsNotNull(resolver);
            Assert.IsInstanceOfType(resolver, typeof(Func<object>));
            Assert.IsInstanceOfType(resolver(), typeof(object));
            Assert.AreNotSame(instance, resolver());
        }
    }
}
