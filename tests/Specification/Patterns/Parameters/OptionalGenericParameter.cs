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

        // https://github.com/unitycontainer/container/issues/294
        [PatternTestMethod("OptionalGenericParameter(T) preserves annotated contract"), WorkItem(294)]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(OptionalGenericParameter))]
        public void OptionalGenericParameter(Type type, Type definition, string member, string import,
                                     Func<object, InjectionMember> func, object registered, object named,
                                     object injected, object @default, bool isNamed)
#if BEHAVIOR_V4 || BEHAVIOR_V5
            => Assert_Optional_Injected(definition, type,
                func(new OptionalGenericParameter(TDependency)), @default, import, isNamed, registered, registered);
#else
            => Assert_Optional_Injected(definition, type,
                func(new OptionalGenericParameter(TDependency)), @default, import, isNamed, registered, named);
#endif


        [PatternTestMethod("OptionalGenericParameter(T, null) forces contract: T, null")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(OptionalGenericParameter))]
        public void OptionalGenericParameter_Null(Type type, Type definition, string member, string import,
                                     Func<object, InjectionMember> func, object registered, object named,
                                     object injected, object @default, bool isNamed)
            => Assert_Optional_Injected(definition, type,
                func(new OptionalGenericParameter(TDependency, null)), @default, import, isNamed, registered, registered);


        [PatternTestMethod("OptionalGenericParameter(T, Name) forces contract: T, Name")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(OptionalGenericParameter))]
        public void OptionalGenericParameter_Name(Type type, Type definition, string member, string import,
                                     Func<object, InjectionMember> func, object registered, object named,
                                     object injected, object @default, bool isNamed)
            => Assert_Optional_Injected(definition, type,
                func(new OptionalGenericParameter(TDependency, Name)), @default, import, isNamed, named, named);


        [PatternTestMethod("OptionalGenericParameter(T[]) on Array")]
        [DynamicData(nameof(Array_Parameters_Data)), TestProperty(PARAMETER, nameof(OptionalGenericParameter))]
        public void OptionalGenericParameter_OnArray(Type type, Type definition, string member, string import,
                                             Func<object, InjectionMember> func, object registered, object named,
                                             object injected, object @default, bool isNamed)
        {
            Container.RegisterType(null, definition, null, null, func(new OptionalGenericParameter(TDependency + "[]")));

            // Register missing types
            RegisterTypes();

            // Act
            var target = definition.MakeGenericType(type);
            var instance = Container.Resolve(target, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance.Value, type.MakeArrayType());
        }


        [PatternTestMethod("OptionalGenericParameter(T()) on Array")]
        [DynamicData(nameof(Array_Parameters_Data)), TestProperty(PARAMETER, nameof(OptionalGenericParameter))]
        public void OptionalGenericParameter_Brackets(Type type, Type definition, string member, string import,
                                             Func<object, InjectionMember> func, object registered, object named,
                                             object injected, object @default, bool isNamed)
        {
            Container.RegisterType(null, definition, null, null, func(new OptionalGenericParameter(TDependency + "()")));

            // Register missing types
            RegisterTypes();

            // Act
            var target = definition.MakeGenericType(type);
            var instance = Container.Resolve(target, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance.Value, type.MakeArrayType());
        }

        #endregion


        #region Failing

        [Ignore("Validation")]
        [PatternTestMethod("OptionalGenericParameter(T[]) on regular type")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(OptionalGenericParameter))]
#if BEHAVIOR_V4
        [ExpectedException(typeof(InvalidOperationException))]
#else
        [ExpectedException(typeof(ResolutionFailedException))]
#endif
        public void OptionalGenericParameter_NoArray(Type type, Type definition, string member, string import,
                                             Func<object, InjectionMember> func, object registered, object named,
                                             object injected, object @default, bool isNamed)
            => Assert_Generic_Injected(definition, type,
                func(new OptionalGenericParameter(TDependency + "[]")), import, isNamed, registered, named);

        [Ignore("Validation")]
        [PatternTestMethod("OptionalGenericParameter(WrongTypeName) throws on resolve")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(OptionalGenericParameter))]
#if BEHAVIOR_V4
        [ExpectedException(typeof(InvalidOperationException))]
#else
        [ExpectedException(typeof(ResolutionFailedException))]
#endif
        public void OptionalGenericParameter_NoMatch(Type type, Type definition, string member, string import,
                                     Func<object, InjectionMember> func, object registered, object named,
                                     object injected, object @default, bool isNamed)
            => Assert_Generic_Injected(definition, type,
                func(new OptionalGenericParameter(TDependency + "[]")), import, isNamed, registered, named);

        [Ignore("Validation")]
        [PatternTestMethod("OptionalGenericParameter(T) on incompatible type")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(OptionalGenericParameter))]
        public void OptionalGenericParameter_Incompatible(Type type, Type definition, string member, string import,
                                                  Func<object, InjectionMember> func, object registered, object named,
                                                  object injected, object @default, bool isNamed)
        {
            var target = _resolvedTestType ??= GetTestType("ObjectTestType", import, member);
            var inject = func(new OptionalGenericParameter(TDependency));

#if BEHAVIOR_V4 || BEHAVIOR_V5
            Assert.ThrowsException<InvalidOperationException>(() 
                => Container.RegisterType(null, target, null, null, inject));
#else
            // No validation during registration 
            Container.RegisterType(null, target, null, null, inject);

            Assert.ThrowsException<ResolutionFailedException>(() => Container.Resolve(target, null));
            RegisterTypes();    // Register missing types

            // Act
            Assert.ThrowsException<ResolutionFailedException>(() => Container.Resolve(target, null));
#endif
        }


        [PatternTestMethod("GenericParameter(null) throws"), TestProperty(PARAMETER, nameof(GenericParameter))]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void OptionalResolvedGeneric_NoName()
        {
            _ = new OptionalGenericParameter(null);
        }

        #endregion


        #region Implementation

        private void Assert_Optional_Injected(Type definition, Type type, InjectionMember member, object @default, string import, bool isNamed, object registered, object named)
        {
            Container.RegisterType(null, definition, null, null, member);

            var target = definition.MakeGenericType(type);

            // Act
            var instance = Container.Resolve(target, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(@default, instance.Value);

            // Register missing types
            RegisterTypes();

            // Act
            instance = Container.Resolve(target, null) as PatternBaseType;
            var expected = !IMPORT_IMPLICIT.Equals(import) && isNamed ? named : registered;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }

        #endregion
    }
}
