using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
#endif

namespace Dependency
{
    public abstract partial class Pattern
    {
        #region Value

        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Member_ByValue(string test, Type type, object defaultValue,
                                           object defaultAttr, object registered, object named,
                                           object injected, object overridden, object @default) 
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                MemberOverride(overridden), overridden);

        #endregion


        #region Injection Members

        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Member_ByInjectionParameter(string test, Type type, object defaultValue,
                                                        object defaultAttr, object registered, object named,
                                                        object injected, object overridden, object @default) 
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                MemberOverride(new InjectionParameter(overridden)), overridden);


        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        [ExpectedException(typeof(ResolutionFailedException))]
        public virtual void Member_ByParameterRecursive(string test, Type type, object defaultValue,
                                                        object defaultAttr, object registered, object named,
                                                        object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                MemberOverride(new InjectionParameter(new InjectionParameter(overridden))), overridden);

        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Member_ByResolvedMember(string test, Type type, object defaultValue,
                                                     object defaultAttr, object registered, object named,
                                                     object injected, object overridden, object @default) 
            => Assert_UnregisteredThrows_RegisteredSuccess(
                BaselineTestType.MakeGenericType(type),
                MemberOverride(new ResolvedParameter(type)), registered);

        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Member_ByResolvedNamed(string test, Type type, object defaultValue,
                                                     object defaultAttr, object registered, object named,
                                                     object injected, object overridden, object @default) 
            => Assert_UnregisteredThrows_RegisteredSuccess(
                BaselineTestType.MakeGenericType(type),
                MemberOverride(new ResolvedParameter(type, Name)),
                named);

        #endregion

        
        #region With Type

        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Parameter_Override_ByType(string test, Type type, object defaultValue,
                                                      object defaultAttr, object registered, object named,
                                                      object injected, object overridden, object @default)
            => Assert_Consumer(type, MemberOverride_ByType(type, overridden), overridden, overridden);


        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Parameter_Override_ByNameType(string test, Type type, object defaultValue,
                                                          object defaultAttr, object registered, object named,
                                                          object injected, object overridden, object @default)
            => Assert_Consumer(type, MemberOverride_ByContract(type, overridden), overridden, overridden);

        #endregion


        #region On Type

        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Member_OnType(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type,
                MemberOverride(overridden).OnType(BaselineTestType.MakeGenericType(type)),
                overridden, named);


        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Member_OnType_Named(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_Consumer(type,
                MemberOverride(overridden).OnType(BaselineTestNamed.MakeGenericType(type)),
                registered, overridden);


        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Member_OnType_NoMatch(string test, Type type, object defaultValue,
                                                                  object defaultAttr, object registered, object named,
                                                                  object injected, object overridden, object @default)
            => Assert_UnregisteredThrows_RegisteredSuccess(
                BaselineTestType.MakeGenericType(type),
                MemberOverride(new ResolvedParameter(type, Name)).OnType(type),
                registered);


        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Parameter_Override_OnType_ByType(string test, Type type, object defaultValue,
                                                             object defaultAttr, object registered, object named,
                                                             object injected, object overridden, object @default)
            => Assert_Consumer(type, 
                MemberOverride_ByType(type, overridden).OnType(BaselineTestNamed.MakeGenericType(type)),
                registered, overridden);
        

        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Parameter_Override_OnType_ByNameType(string test, Type type, object defaultValue,
                                                          object defaultAttr, object registered, object named,
                                                          object injected, object overridden, object @default)
            => Assert_Consumer(type, 
                MemberOverride_ByContract(type, overridden).OnType(BaselineTestNamed.MakeGenericType(type)), 
                registered, overridden);

        #endregion
    }
}
