using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Resolution;
#endif

namespace Dependency
{
    public abstract partial class Pattern
    {
        #region Type

        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public void Dependency_ByType(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                new DependencyOverride(type, overridden),
                overridden);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public void Dependency_ByType_Named(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestNamed.MakeGenericType(type),
                new DependencyOverride(type, overridden), overridden);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public void Dependency_ByType_NoMatch(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_FixtureBaseType(BaselineTestType.MakeGenericType(type),
                           new DependencyOverride(typeof(Pattern), overridden),
                           registered, @default);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public void Dependency_ByType_InGraph(string test, Type type, object defaultValue,
                                                              object defaultAttr, object registered, object named,
                                                              object injected, object overridden, object @default)
            => Assert_Consumer(type, new DependencyOverride(type, overridden), overridden, overridden);

        #endregion


        #region Name

#if !UNITY_V4

#if !BEHAVIOR_V5
        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public void Dependency_ByNullName(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                              new DependencyOverride((string)null, overridden), overridden);
#endif

        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_ByName(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestNamed.MakeGenericType(type),
                              new DependencyOverride(Name, overridden), overridden);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public void Dependency_ByName_NoMatch(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_FixtureBaseType(BaselineTestType.MakeGenericType(type),
                           new DependencyOverride(Name, overridden),
                           registered, @default);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_ByName_InGraph(string test, Type type, object defaultValue,
                                                              object defaultAttr, object registered, object named,
                                                              object injected, object overridden, object @default)
            => Assert_Consumer(type, new DependencyOverride((string)null, overridden),
                                     overridden, named);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_ByName_InReverse(string test, Type type, object defaultValue,
                                                                    object defaultAttr, object registered, object named,
                                                                    object injected, object overridden, object @default)
            => Assert_Consumer(type, new DependencyOverride(Name, overridden),
                                     registered, overridden);
#endif
        #endregion


        #region Contract

#if !UNITY_V4
        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public void Dependency_ByContract(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                              new DependencyOverride(type, null, overridden), overridden);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_ByContract_Named(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestNamed.MakeGenericType(type),
                              new DependencyOverride(type, Name, overridden), overridden);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public void Dependency_ByContract_NoMatch(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_FixtureBaseType(BaselineTestType.MakeGenericType(type),
                           new DependencyOverride(type, Name, overridden),
                           registered, @default);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_ByContract_InGraph(string test, Type type, object defaultValue,
                                                              object defaultAttr, object registered, object named,
                                                              object injected, object overridden, object @default)
            => Assert_Consumer(type, new DependencyOverride(type, null, overridden),
                                     overridden, named);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_ByContract_InReverse(string test, Type type, object defaultValue,
                                                                    object defaultAttr, object registered, object named,
                                                                    object injected, object overridden, object @default)
            => Assert_Consumer(type, new DependencyOverride(type, Name, overridden),
                                     registered, overridden);
#endif
        #endregion


        #region Target
#if !UNITY_V4
        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_ByTarget(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default) 
            => Assert_Consumer(type, 
                new DependencyOverride(BaselineTestType.MakeGenericType(type), type, null, overridden), 
                overridden, named);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_ByTarget_Named(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type,
                new DependencyOverride(BaselineTestNamed.MakeGenericType(type), type, Name, overridden),
                registered, overridden);

        
        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_ByTarget_NoMatch(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type,
                new DependencyOverride(BaselineTestType.MakeGenericType(type), type, Name, overridden),
                registered, named);
#endif
        #endregion


        #region OnType
#if !UNITY_V4
        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_OnType(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type,
                new DependencyOverride(type, null, overridden).OnType(BaselineTestType.MakeGenericType(type)),
                overridden, named);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_OnType_Named(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type,
                new DependencyOverride(type, Name, overridden).OnType(BaselineTestNamed.MakeGenericType(type)),
                registered, overridden);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_OnType_NoMatch(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type,
                new DependencyOverride(type, Name, overridden).OnType(BaselineTestType.MakeGenericType(type)),
                registered, named);
#endif
        #endregion
    }
}
