using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Injection
{
    public abstract partial class Pattern
    {
        [TestCategory(CATEGORY_INJECT)]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Inject_Value(string test, Type type, object defaultValue, object defaultAttr,
                                           object registered, object named, object injected, object overridden,
                                           object @default) 
            => Assert_AlwaysSuccessful(
                BaselineTestType.MakeGenericType(type), 
                InjectionMember_Value(injected), 
                injected, injected);

        [TestCategory(CATEGORY_INJECT)]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Inject_Named(string test, Type type, object defaultValue, object defaultAttr,
                                         object registered, object named, object injected, object overridden,
                                         object @default)
            => Assert_AlwaysSuccessful(
                BaselineTestNamed.MakeGenericType(type),
                InjectionMember_Value(injected),
                injected, injected);


        [TestCategory(CATEGORY_INJECT)]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Inject_WithType(string test, Type type, object defaultValue, object defaultAttr,
                                            object registered, object named, object injected, object overridden,
                                            object @default)
            => Assert_Injection(
                BaselineTestType.MakeGenericType(type),
                InjectionMember_Value(type), @default, registered);

#if BEHAVIOR_V4 || BEHAVIOR_V5
        [TestCategory(CATEGORY_INJECT)]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
#if UNITY_V4 || UNITY_V5
        [ExpectedException(typeof(InvalidOperationException))]
#else
        [ExpectedException(typeof(ResolutionFailedException))]
#endif
        public virtual void Inject_NoMatch(string test, Type type, object defaultValue, object defaultAttr,
                                            object registered, object named, object injected, object overridden,
                                            object @default)
        {
            var target = BaselineConsumer.MakeGenericType(type);
            var inject = InjectionMember_Value(injected);
            Container.RegisterType(null, target, null, null, inject);

            // Register missing types
            RegisterTypes();

            // Act
            _ = Container.Resolve(target, null);
        }
#endif


#if !BEHAVIOR_V4 && !BEHAVIOR_V5 && !BEHAVIOR_V6
        [TestCategory(CATEGORY_INJECT)]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Inject_Array(string test, Type type, object defaultValue, object defaultAttr,
                                         object registered, object named, object injected, object overridden,
                                         object @default)
        {
            var instance = Array.CreateInstance(type, 1);
            ((IList)instance)[0] = @default;

            Assert_Array_Import(BaselineArrayType.MakeGenericType(type), InjectionMember_Value(instance), instance);
        }


        [TestCategory(CATEGORY_INJECT)]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Inject_Enumerable(string test, Type type, object defaultValue, object defaultAttr, 
                                              object registered, object named, object injected, object overridden, 
                                              object @default)
        {
            var instance = Array.CreateInstance(type, 1);
            ((IList)instance)[0] = @default;

            var target = BaselineTestType.MakeGenericType(typeof(IEnumerable<>).MakeGenericType(type));

            Assert_AlwaysSuccessful(target, InjectionMember_Value(instance), instance, instance);
        }

#endif

#if !UNITY_V4 && !UNITY_V5 && !UNITY_V6

        [TestCategory(CATEGORY_INJECT)]
        [DataTestMethod, DynamicData(nameof(Type_Compatibility_Data), typeof(PatternBase))]
        public virtual void Inject_Resolver(string test, Type type, object defaultValue, object defaultAttr,
                                            object registered, object named, object injected, object overridden,
                                            object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                               InjectionMember_Value(new ValidatingResolver(injected)),
                               injected, injected);
        [TestCategory(CATEGORY_INJECT)]
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Inject_TypeFactory(string test, Type type, object defaultValue, object defaultAttr,
                                               object registered, object named, object injected, object overridden,
                                               object @default)
            => Assert_AlwaysSuccessful(BaselineTestType.MakeGenericType(type),
                               InjectionMember_Value(new ValidatingResolverFactory(injected)),
                               injected, injected);
#endif


#if BEHAVIOR_V4 || BEHAVIOR_V5
        [TestCategory(CATEGORY_INJECT)]
#if !BEHAVIOR_V4 && !BEHAVIOR_V5
        [ExpectedException(typeof(ResolutionFailedException))]
#endif
        [DataTestMethod, DynamicData(nameof(Test_Type_Data), typeof(PatternBase))]
        public virtual void Inject_NoPublicMember(string test, Type type, object defaultValue, object defaultAttr,
                                                   object registered, object named, object injected, object overridden,
                                                   object @default)
#if BEHAVIOR_V4
        {
            // Unity v4 ignores injection members for nonpublic members
        }
#else
            => Assert_Fail(NoPublicMember.MakeGenericType(type), InjectionMember_Value(injected));
#endif
#endif
    }
}
