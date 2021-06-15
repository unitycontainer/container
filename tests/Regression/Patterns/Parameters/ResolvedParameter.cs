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

#if !UNITY_V4
        [PatternTestMethod("Ctor() preserves annotated contract"), TestProperty(PARAMETER, nameof(ResolvedParameter))]
        [DynamicData(nameof(Parameters_Test_Data))]
        public void ResolvedParameter(Type type, Type definition, string member, string import,
                                      Func<object, InjectionMember> func, object registered, object named,
                                      object injected, object @default, bool isNamed)
            => Assert_Parameter_Injected(definition.MakeGenericType(type),
                func(new ResolvedParameter()), import, isNamed, registered, named);
#endif
        [PatternTestMethod("Ctor(type) forces contract: type, null")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(ResolvedParameter))]
        public void ResolvedParameter_Type(Type type, Type definition, string member, string import,
                                           Func<object, InjectionMember> func, object registered, object named,
                                           object injected, object @default, bool isNamed)
            => Assert_Parameter_Injected(definition.MakeGenericType(type),
                func(new ResolvedParameter(type)), import, isNamed, registered, registered);


#if !UNITY_V4
        [PatternTestMethod("Ctor(null) forces contract: AnnotatedType, null")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(ResolvedParameter))]
        public void ResolvedParameter_Null(Type type, Type definition, string member, string import,
                                           Func<object, InjectionMember> func, object registered, object named,
                                           object injected, object @default, bool isNamed)
            => Assert_Parameter_Injected(definition.MakeGenericType(type),
                func(new ResolvedParameter((string)null)), import, isNamed, registered, registered);


        [PatternTestMethod("Ctor(Name) forces contract: AnnotatedType, Name")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(ResolvedParameter))]
        public void ResolvedParameter_Name(Type type, Type definition, string member, string import,
                                           Func<object, InjectionMember> func, object registered, object named,
                                           object injected, object @default, bool isNamed)
            => Assert_Parameter_Injected(definition.MakeGenericType(type),
                func(new ResolvedParameter(Name)), import, isNamed, named, named);
#endif

        [PatternTestMethod("Ctor(type, null) forces contract: type, null")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(ResolvedParameter))]
        public void ResolvedParameter_Type_Null(Type type, Type definition, string member, string import,
                                                Func<object, InjectionMember> func, object registered, object named,
                                                object injected, object @default, bool isNamed)
            => Assert_Parameter_Injected(definition.MakeGenericType(type),
                func(new ResolvedParameter(type, null)), import, isNamed, registered, registered);


        [PatternTestMethod("Ctor(type, Name) forces contract: type, Name")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(ResolvedParameter))]
        public void ResolvedParameter_Type_Name(Type type, Type definition, string member, string import,
                                                Func<object, InjectionMember> func, object registered, object named,
                                                object injected, object @default, bool isNamed)
            => Assert_Parameter_Injected(definition.MakeGenericType(type),
                func(new ResolvedParameter(type, Name)), import, isNamed, named, named);

        #endregion


        #region Implementation

        private void Assert_Parameter_Injected(Type type, InjectionMember member, string import, bool isNamed, object registered, object named)
        {
            Container.RegisterType(null, type, null, null, member);

            // Validate
            Assert.ThrowsException<ResolutionFailedException>(() => Container.Resolve(type, null));

            // Register missing types
            RegisterTypes();

            // Act
            var instance = Container.Resolve(type, null) as PatternBaseType;
            var expected = !IMPORT_IMPLICIT.Equals(import) && isNamed ? named : registered;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }

        #endregion
    }
}
