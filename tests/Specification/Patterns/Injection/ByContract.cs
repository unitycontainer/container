using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Injection
{
    public abstract partial class Pattern
    {
#if !UNITY_V4
        [DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        [PatternTestMethod("Inject Unnamed dependency by XxxMember()"), TestCategory(CATEGORY_INJECT)]
        public virtual void Inject_Default(string test, Type type, object defaultValue, object defaultAttr,
                                           object registered, object named, object injected, object overridden,
                                           object @default)
            => Assert_Injection(
                BaselineTestType.MakeGenericType(type),
                InjectionMember_Default(), 
                @default, registered);


        [DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        [PatternTestMethod("Inject Named dependency by XxxMember()"), TestCategory(CATEGORY_INJECT)]
        public virtual void Inject_Named_Default(string test, Type type, object defaultValue, object defaultAttr,
                                           object registered, object named, object injected, object overridden,
                                           object @default)
            => Assert_Injection(
                BaselineTestNamed.MakeGenericType(type),
                InjectionMember_Default(), 
                @default, named);
#endif

        [DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        [PatternTestMethod("Inject {1} dependency by {2} and {3}"), TestCategory(CATEGORY_INJECT)]
        public virtual void Inject_Unnamed_Type_Null(string test, Type type, object defaultValue, object defaultAttr,
                                           object registered, object named, object injected, object overridden,
                                           object @default) 
            => Assert_Injection(
                   BaselineTestType.MakeGenericType(type),
                   InjectionMember_Contract(type, null), 
                   @default, registered);


        [DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        [PatternTestMethod("Inject {1} dependency by {2} and {3}"), TestCategory(CATEGORY_INJECT)]
        public virtual void Inject_Named_Type_Null(string test, Type type, object defaultValue, object defaultAttr,
                                           object registered, object named, object injected, object overridden,
                                           object @default) 
            => Assert_Injection(
                   BaselineTestNamed.MakeGenericType(type),
                   InjectionMember_Contract(type, null),
                   @default, registered);


        
        [DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        [PatternTestMethod("Inject {1} dependency by {2} and {3}"), TestCategory(CATEGORY_INJECT)]
        public virtual void Inject_Unnamed_Type_Name(string test, Type type, object defaultValue, object defaultAttr,
                                              object registered, object named, object injected, object overridden,
                                              object @default) 
            => Assert_Injection(
                BaselineTestType.MakeGenericType(type), 
                InjectionMember_Contract(type, Name), 
                @default, named);

        [DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        [PatternTestMethod("Inject {1} dependency by {2} and {3}"), TestCategory(CATEGORY_INJECT)]
        public virtual void Inject_Named_Type_Name(string test, Type type, object defaultValue, object defaultAttr,
                                              object registered, object named, object injected, object overridden,
                                              object @default)
            => Assert_Injection(
                BaselineTestType.MakeGenericType(type),
                InjectionMember_Contract(type, Name),
                @default, named);
    }
}
