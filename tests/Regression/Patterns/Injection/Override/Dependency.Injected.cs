using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity.Resolution;
#endif

namespace Injection
{
    public abstract partial class Pattern 
    {
        #region Type

#if !UNITY_V4
        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_Injected_ByType(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                               InjectionMember_Value(new ResolvedParameter()),
                               new DependencyOverride(type, overridden), overridden);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_Injected_ByType_Named(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestNamed.MakeGenericType(type),
                               InjectionMember_Value(new ResolvedParameter(Name)),
                               new DependencyOverride(type, overridden), overridden);
#endif

        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_Injected_ByType_NoMatch(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                              InjectionMember_Value(new InjectionParameter(injected)),
                              new DependencyOverride(typeof(Pattern), overridden),
                              injected);
        #endregion


        #region Name

#if !UNITY_V4
        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_Injected_ByName(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                              InjectionMember_Value(injected),
                              new DependencyOverride((string)null, overridden), overridden);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_Injected_ByName_Named(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestNamed.MakeGenericType(type),
                               InjectionMember_Value(new ResolvedParameter(Name)),
                               new DependencyOverride(Name, overridden), overridden);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_Injected_ByName_NoMatch(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
        {
            var target = BaselineTestNamed.MakeGenericType(type);

            // Arrange
            Container.RegisterType(null, target, null, null, InjectionMember_Value(new ResolvedParameter(Name)));

            // Register missing types
            RegisterTypes();

            // Act
            var instance = Container.Resolve(target, null, new DependencyOverride((string)null, overridden)) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(named, instance.Value);
        }

#endif
        #endregion


        #region Contract

#if !UNITY_V4
        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public void Dependency_Injected_ByContract(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                               InjectionMember_Value(new ResolvedParameter()),
                               new DependencyOverride(type, null, overridden), overridden);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Dependency_Injected_ByContract_Named(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestNamed.MakeGenericType(type),
                               InjectionMember_Value(new ResolvedParameter(Name)),
                               new DependencyOverride(type, Name, overridden), overridden);


        [TestProperty(OVERRIDE, nameof(DependencyOverride))]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public void Dependency_Injected_ByContract_Ignored(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_FixtureBaseType(BaselineTestType.MakeGenericType(type),
                           new DependencyOverride(type, Name, overridden),
                           registered, @default);
#endif
        #endregion
    }
}
