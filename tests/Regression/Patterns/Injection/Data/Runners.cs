using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity;
#endif

namespace Injection
{
    public abstract partial class Pattern
    {
        protected virtual void Assert_Injection(Type type, InjectionMember member, object @default, object expected)
        {
            // Inject
            Container.RegisterType(null, type, null, null, member);

            // Act
            Assert.ThrowsException<ResolutionFailedException>(() => Container.Resolve(type, null));

            // Register missing types
            RegisterTypes();

            // Act
            var instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }
    }
}
