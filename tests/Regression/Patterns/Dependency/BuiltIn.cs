using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections.Generic;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Dependency
{
    public abstract partial class Pattern
    {
        [DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        [PatternTestMethod("{0} dependency as {1}"), TestCategory(CATEGORY_DEPENDENCY)] 
        public virtual void Unnamed_Array(string test, Type type, object defaultValue, object defaultAttr,
                                                                 object registered, object named,
                                                                 object injected, object overridden,
                                                                 object @default)
            => Assert_Array_Import(BaselineTestType.MakeGenericType(type.MakeArrayType()));


        [DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        [PatternTestMethod("{0} dependency as {1}"), TestCategory(CATEGORY_DEPENDENCY)]
        public virtual void Named_Array(string test, Type type, object defaultValue, object defaultAttr,
                                                                 object registered, object named,
                                                                 object injected, object overridden,
                                                                 object @default)
            => Assert_Array_Import(BaselineTestType.MakeGenericType(type.MakeArrayType()));


#if !BEHAVIOR_V4
        [DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        [PatternTestMethod("{0} dependency as {1}"), TestCategory(CATEGORY_DEPENDENCY)]
        public virtual void Unnamed_Enumerable(string test, Type type, object defaultValue, object defaultAttr, object registered,
                                               object named, object injected, object overridden, object @default)
            => Assert_Enumerable_Import(BaselineTestType.MakeGenericType(typeof(IEnumerable<>).MakeGenericType(type)));
#endif


        [DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        [PatternTestMethod("{0} dependency as {1}"), TestCategory(CATEGORY_DEPENDENCY)]
        public virtual void Unnamed_Lazy(string test, Type type, object defaultValue, object defaultAttr, object registered,
                                 object named, object injected, object overridden, object @default)
            => Assert_Lazy_Import(
                BaselineTestType.MakeGenericType(typeof(Lazy<>).MakeGenericType(type)), registered);


        [DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        [PatternTestMethod("{0} dependency as {1}"), TestCategory(CATEGORY_DEPENDENCY)]
        public virtual void Unnamed_Func(string test, Type type, object defaultValue, object defaultAttr, object registered,
                                 object named, object injected, object overridden, object @default)
            => Assert_Func_Import(
                BaselineTestType.MakeGenericType(typeof(Func<>).MakeGenericType(type)), registered);


        [DynamicData(nameof(BuiltInTypes_Data), typeof(PatternBase))]
        [PatternTestMethod("{0} dependency as IUnityContainer"), TestCategory(CATEGORY_DEPENDENCY)]
        public virtual void Unnamed_BuiltIn_Interface(string test, Type type)
            => Assert_ResolutionSuccess(BaselineTestType.MakeGenericType(type));


        [DynamicData(nameof(BuiltInTypes_Data), typeof(PatternBase))]
        [ExpectedException(typeof(ResolutionFailedException))]
        [PatternTestMethod("{0} dependency as IUnityContainer"), TestCategory(CATEGORY_DEPENDENCY)]
        public virtual void Named_BuiltIn_Interface(string test, Type type) 
            => _ = Container.Resolve(type, Name) as PatternBaseType;
    }
}
