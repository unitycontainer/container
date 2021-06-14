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

namespace Dependency.Optional
{
    public abstract partial class Pattern : Dependency.Pattern
    {
        #region Dependency

        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public override void Unnamed_ByType(string test, Type type,
                                       object defaultValue, object defaultAttr,
                                       object registered, object named,
                                       object injected, object overridden,
                                       object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                @default, registered);

        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public override void Named_ByType(string test, Type type,
                                       object defaultValue, object defaultAttr,
                                       object registered, object named,
                                       object injected, object overridden,
                                       object @default)
            => Assert_AlwaysSuccessful(BaselineTestNamed.MakeGenericType(type),
                @default, named);

        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public override void InInherited_Type(string test, Type type,
                                                         object defaultValue, object defaultAttr,
                                                         object registered, object named,
                                                         object injected, object overridden,
                                                         object @default)
            => Assert_AlwaysSuccessful(CorrespondingTypeDefinition.MakeGenericType(type),
                @default, registered);


        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public override void Twice_InInherited_Type(string test, Type type,
                                                         object defaultValue, object defaultAttr,
                                                         object registered, object named,
                                                         object injected, object overridden,
                                                         object @default)
            => Assert_AlwaysSuccessful(CorrespondingTypeDefinition.MakeGenericType(type),
                @default, registered);

        #endregion


        #region Runners

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
        
        #endregion
    }
}
