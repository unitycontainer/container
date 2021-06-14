using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
using Unity.Resolution;
#endif

namespace Injection.Optional
{
    public abstract partial class Pattern : Injection.Pattern
    {
        protected override void Assert_UnregisteredThrows_RegisteredSuccess(Type type, ResolverOverride @override, object expected)
        {
            Container.RegisterType(null, type, null, null);

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
