using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
#endif

namespace Injection
{
    public abstract partial class Pattern 
    {
        #region Value

        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Member_Injected_ByValue(string test, Type type, object defaultValue,
                                           object defaultAttr, object registered, object named,
                                           object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                InjectionMember_Value(new InjectionParameter(injected)),
                MemberOverride(overridden), overridden);

        #endregion


        #region Injection Members

        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Member_Injected_ByInjectionParameter(string test, Type type, object defaultValue,
                                                        object defaultAttr, object registered, object named,
                                                        object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                InjectionMember_Value(new InjectionParameter(injected)),
                MemberOverride(new InjectionParameter(overridden)), overridden);


        [TestProperty(OVERRIDE, MEMBER_OVERRIDE)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        [ExpectedException(typeof(ResolutionFailedException))]
        public virtual void Member_Injected_ByParameterRecursive(string test, Type type, object defaultValue,
                                                        object defaultAttr, object registered, object named,
                                                        object injected, object overridden, object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                InjectionMember_Value(new InjectionParameter(injected)),
                MemberOverride(new InjectionParameter(new InjectionParameter(overridden))), overridden);

        #endregion
    }
}
