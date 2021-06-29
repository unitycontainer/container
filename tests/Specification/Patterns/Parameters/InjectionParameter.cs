using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
#endif


namespace Parameters
{
    public partial class Pattern
    {
        #region Success

        [PatternTestMethod("InjectionParameter(value)"), TestProperty(PARAMETER, nameof(InjectionParameter))]
        [DynamicData(nameof(Parameters_Test_Data))]
        public void InjectionParameter_Value(Type type, Type definition, string member, string annotatation,
                                             Func<object, InjectionMember> func, object registered, object named,
                                             object injected,   object @default, bool   isNamed) 
            => Assert_AlwaysSuccessful(definition.MakeGenericType(type),
                                       func(new InjectionParameter(injected)),
                                       injected, injected);


        [PatternTestMethod("InjectionParameter(type, value)"), TestProperty(PARAMETER, nameof(InjectionParameter))]
        [DynamicData(nameof(Parameters_Test_Data))]
        public void InjectionParameter_Type_Value(Type type, Type definition, string member, string annotatation,
                                                  Func<object, InjectionMember> func, object registered, object named,
                                                  object injected, object @default, bool isNamed)
            => Assert_AlwaysSuccessful(definition.MakeGenericType(type),
                                       func(new InjectionParameter(type, injected)),
                                       injected, injected);

#if !BEHAVIOR_V4
        [PatternTestMethod("InjectionParameter(default(T))"), TestProperty(PARAMETER, nameof(InjectionParameter))]
        [DynamicData(nameof(Parameters_Test_Data))]
        public void InjectionParameter_Null(Type type, Type definition, string member, string annotatation,
                                            Func<object, InjectionMember> func, object registered, object named,
                                            object injected, object @default, bool isNamed)
            => Assert_AlwaysSuccessful(definition.MakeGenericType(type),
                                       func(new InjectionParameter(@default)),
                                       @default, @default);
#endif


        [PatternTestMethod("InjectionParameter(type, default(T))"), TestProperty(PARAMETER, nameof(InjectionParameter))]
        [DynamicData(nameof(Parameters_Test_Data))]
        public void InjectionParameter_Type_Null(Type type, Type definition, string member, string annotatation,
                                                 Func<object, InjectionMember> func, object registered, object named,
                                                 object injected, object @default, bool isNamed)
            => Assert_AlwaysSuccessful(definition.MakeGenericType(type),
                                       func(new InjectionParameter(type, @default)),
                                       @default, @default);
        #endregion


        #region Failing

        [PatternTestMethod("InjectionParameter(incompatible)"), TestProperty(PARAMETER, nameof(InjectionParameter))]
        [DynamicData(nameof(Parameters_Test_Data))]
#if BEHAVIOR_V4
        [ExpectedException(typeof(InvalidOperationException))]
#else
        [ExpectedException(typeof(ResolutionFailedException))]
#endif
        public void InjectionParameter_Incompatible(Type type, Type definition, string member, string annotatation,
                                                    Func<object, InjectionMember> func, object registered, object named, 
                                                    object injected, object @default, bool isNamed)
            => Assert_AlwaysSuccessful(definition.MakeGenericType(type),
                                       func(new InjectionParameter(false)),
                                       injected, injected);


        [PatternTestMethod("InjectionParameter(type, incompatible)"), TestProperty(PARAMETER, nameof(InjectionParameter))]
        [ExpectedException(typeof(ResolutionFailedException)), DynamicData(nameof(Parameters_Test_Data))]
        public void InjectionParameter_Type_Incompatible(Type type, Type definition, string member, string annotatation,
                                                         Func<object, InjectionMember> func, object registered, object named,
                                                         object injected, object @default, bool isNamed)
            => Assert_AlwaysSuccessful(definition.MakeGenericType(type),
                                       func(new InjectionParameter(type, false)),
                                       injected, injected);


        [PatternTestMethod("InjectionParameter(null, value)"), TestProperty(PARAMETER, nameof(InjectionParameter))]
#if !BEHAVIOR_V4
        [ExpectedException(typeof(ArgumentNullException))]
#endif
        public void InjectionParameter_Null_Value() => _ = new InjectionParameter(null, this);

        #endregion
    }
}
