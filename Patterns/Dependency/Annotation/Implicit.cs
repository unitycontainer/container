using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Resolution;
#endif

namespace Dependency.Implicit
{
    public abstract partial class Pattern : Dependency.Pattern
    {
        #region Ignored

        public override void Named_ByType(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }

        #endregion


        #region DependencyOverride
#if !UNITY_V4
        #region Name

        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public override void Dependency_ByName(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_UnregisteredThrows_RegisteredSuccess(
                BaselineTestNamed.MakeGenericType(type),
                new DependencyOverride(Name, overridden),
                registered);


        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public override void Dependency_ByName_InGraph(string test, Type type, object defaultValue,
                                                              object defaultAttr, object registered, object named,
                                                              object injected, object overridden, object @default)
            => Assert_Consumer(type,
                new DependencyOverride((string)null, overridden),
                overridden, overridden);


        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public override void Dependency_ByName_InReverse(string test, Type type, object defaultValue,
                                                                    object defaultAttr, object registered, object named,
                                                                    object injected, object overridden, object @default)
            => Assert_Consumer(type,
                new DependencyOverride(Name, overridden),
                registered, registered);

        #endregion


        #region Contract


        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public override void Dependency_ByContract_Named(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_UnregisteredThrows_RegisteredSuccess(
                BaselineTestNamed.MakeGenericType(type),
                new DependencyOverride(type, Name, overridden),
                registered);


        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public override void Dependency_ByContract_InGraph(string test, Type type, object defaultValue,
                                                              object defaultAttr, object registered, object named,
                                                              object injected, object overridden, object @default)
            => Assert_Consumer(type,
                new DependencyOverride(type, null, overridden),
                overridden, overridden);


        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public override void Dependency_ByContract_InReverse(string test, Type type, object defaultValue,
                                                                    object defaultAttr, object registered, object named,
                                                                    object injected, object overridden, object @default)
            => Assert_Consumer(type,
                new DependencyOverride(type, Name, overridden),
                registered, registered);

        #endregion


        #region Target

        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public override void Dependency_ByTarget(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type, new DependencyOverride(BaselineTestType.MakeGenericType(type), type, null, overridden),
                overridden, registered);


        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public override void Dependency_ByTarget_Named(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type, new DependencyOverride(BaselineTestNamed.MakeGenericType(type), type, Name, overridden),
                registered, registered);


        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public override void Dependency_ByTarget_NoMatch(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type, new DependencyOverride(BaselineTestType.MakeGenericType(type), type, Name, overridden),
                registered, registered);

        #endregion


        #region OnType

        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public override void Dependency_OnType(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type, new DependencyOverride(type, null, overridden).OnType(BaselineTestType.MakeGenericType(type)),
                overridden, registered);


        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public override void Dependency_OnType_Named(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type, new DependencyOverride(type, Name, overridden).OnType(BaselineTestNamed.MakeGenericType(type)),
                registered, registered);


        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public override void Dependency_OnType_NoMatch(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type, new DependencyOverride(type, Name, overridden).OnType(BaselineTestType.MakeGenericType(type)),
                registered, registered);

        #endregion
#endif
        #endregion


        #region MemberOverride

        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public override void Member_OnType(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type, MemberOverride(overridden).OnType(BaselineTestType.MakeGenericType(type)),
                overridden, registered);

        public override void Parameter_Override_ByNameType(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }

        #endregion
    }
}
