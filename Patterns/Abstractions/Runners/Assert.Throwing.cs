using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
using Unity.Resolution;
#endif

namespace Regression
{
    public abstract partial class PatternBase
    {
        protected void Assert_UnregisteredThrows_RegisteredSuccess(Type type, object expected)
        {
            // Validate
            Assert.ThrowsException<ResolutionFailedException>(() => Container.Resolve(type, null));

            // Register missing types
            RegisterTypes();

            // Act
            var instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }

        protected void Assert_UnregisteredThrows_RegisteredSuccess(Type type, InjectionMember member, object expected)
        {
            Container.RegisterType(null, type, null, null, member);

            // Validate
            Assert.ThrowsException<ResolutionFailedException>(() => Container.Resolve(type, null));

            // Register missing types
            RegisterTypes();

            // Act
            var instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }

        protected virtual void Assert_UnregisteredThrows_RegisteredSuccess(Type type, ResolverOverride @override, object expected)
        {
            Container.RegisterType(null, type, null, null);

            // Validate
            Assert.ThrowsException<ResolutionFailedException>(() => Container.Resolve(type, null, @override));

            // Register missing types
            RegisterTypes();

            // Act
            var instance = Container.Resolve(type, null, @override) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }
    }
}
