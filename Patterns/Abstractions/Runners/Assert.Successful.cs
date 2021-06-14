using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Resolution;
using Unity.Injection;
using Unity;
#endif

namespace Regression
{
    public abstract partial class PatternBase
    {
        protected void Assert_AlwaysSuccessful(Type type, object @default, object expected)
        {
            // Act
            var instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(@default, instance.Value);
            
            // Register missing types
            RegisterTypes();

            // Act
            instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }
        
        protected void Assert_AlwaysSuccessful(Type type, ResolverOverride @override, object expected)
        {
            // Arrange
            Container.RegisterType(null, type, null, null);

            // Validate
            var instance = Container.Resolve(type, null, @override) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);

            // Register missing types
            RegisterTypes();

            // Act
            instance = Container.Resolve(type, null, @override) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }
        
        protected void Assert_AlwaysSuccessful(Type type, InjectionMember member, object @default, object expected)
        {
            // Inject
            Container.RegisterType(null, type, null, null, member);

            // Act
            var instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(@default, instance.Value);

            // Register missing types
            RegisterTypes();

            // Act
            instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }
        
        protected void Assert_AlwaysSuccessful(Type type, InjectionMember member, ResolverOverride @override, object expected)
        {
            // Arrange
            Container.RegisterType(null, type, null, null, member);

            // Validate
            var instance = Container.Resolve(type, null, @override) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);

            // Register missing types
            RegisterTypes();

            // Act
            instance = Container.Resolve(type, null, @override) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }
    }
}
