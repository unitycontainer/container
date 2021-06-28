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
        #region Fields

        protected Type _resolvedTestType;

        #endregion


        #region Success

        // https://github.com/unitycontainer/container/issues/294
        [PatternTestMethod("GenericParameter(T) preserves annotated contract"), WorkItem(294)]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(GenericParameter))]
        public void GenericParameter(Type type, Type definition, string member, string import,
                                     Func<object, InjectionMember> func, object registered, object named,
                                     object injected, object @default, bool isNamed)
#if BEHAVIOR_V4 || BEHAVIOR_V5
            => Assert_Generic_Injected(definition, type,
                func(new GenericParameter(TDependency)), import, isNamed, registered, registered);
#else
            => Assert_Generic_Injected(definition, type,
                func(new GenericParameter(TDependency)), import, isNamed, registered, named);
#endif

        [PatternTestMethod("GenericParameter(T, null) forces contract: T, null")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(GenericParameter))]
        public void GenericParameter_Null(Type type, Type definition, string member, string import,
                                     Func<object, InjectionMember> func, object registered, object named,
                                     object injected, object @default, bool isNamed)
            => Assert_Generic_Injected(definition, type,
                func(new GenericParameter(TDependency, null)), import, isNamed, registered, registered);


        [PatternTestMethod("GenericParameter(T, Name) forces contract: T, Name")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(GenericParameter))]
        public void GenericParameter_Name(Type type, Type definition, string member, string import,
                                     Func<object, InjectionMember> func, object registered, object named,
                                     object injected, object @default, bool isNamed)
            => Assert_Generic_Injected(definition, type,
                func(new GenericParameter(TDependency, Name)), import, isNamed, named, named);


        [PatternTestMethod("GenericParameter(T[]) on Array")]
        [DynamicData(nameof(Array_Parameters_Data)), TestProperty(PARAMETER, nameof(GenericParameter))]
        public void GenericParameter_OnArray(Type type, Type definition, string member, string import,
                                             Func<object, InjectionMember> func, object registered, object named,
                                             object injected, object @default, bool isNamed)
        {
            Container.RegisterType(null, definition, null, null, func(new GenericParameter(TDependency + "[]")));

            // Register missing types
            RegisterTypes();

            // Act
            var target = definition.MakeGenericType(type);
            var instance = Container.Resolve(target, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance.Value, type.MakeArrayType());
        }


        [PatternTestMethod("GenericParameter(T()) on Array")]
        [DynamicData(nameof(Array_Parameters_Data)), TestProperty(PARAMETER, nameof(GenericParameter))]
        public void GenericParameter_Brackets(Type type, Type definition, string member, string import,
                                             Func<object, InjectionMember> func, object registered, object named,
                                             object injected, object @default, bool isNamed)
        {
            Container.RegisterType(null, definition, null, null, func(new GenericParameter(TDependency + "()")));

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
        [PatternTestMethod("GenericParameter(T[]) on regular type")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(GenericParameter))]
#if BEHAVIOR_V4
        [ExpectedException(typeof(InvalidOperationException))]
#else
        [ExpectedException(typeof(ResolutionFailedException))]
#endif
        public void GenericParameter_NoArray(Type type, Type definition, string member, string import,
                                             Func<object, InjectionMember> func, object registered, object named,
                                             object injected, object @default, bool isNamed)
            => Assert_Generic_Injected(definition, type,
                func(new GenericParameter(TDependency + "[]")), import, isNamed, registered, named);


        [Ignore("Validation")]
        [PatternTestMethod("GenericParameter(WrongTypeName) throws on resolve")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(GenericParameter))]
#if BEHAVIOR_V4
        [ExpectedException(typeof(InvalidOperationException))]
#else
        [ExpectedException(typeof(ResolutionFailedException))]
#endif
        public void GenericParameter_NoMatch(Type type, Type definition, string member, string import,
                                     Func<object, InjectionMember> func, object registered, object named,
                                     object injected, object @default, bool isNamed)
            => Assert_Generic_Injected(definition, type,
                func(new GenericParameter(TDependency + "[]")), import, isNamed, registered, named);

        [Ignore("Validation")]
        [PatternTestMethod("GenericParameter(T) on incompatible type")]
        [DynamicData(nameof(Parameters_Test_Data)), TestProperty(PARAMETER, nameof(GenericParameter))]
        public void GenericParameter_Incompatible(Type type, Type definition, string member, string import,
                                                  Func<object, InjectionMember> func, object registered, object named,
                                                  object injected, object @default, bool isNamed)
        {
            var target = _resolvedTestType ??= GetTestType("ObjectTestType", import, member);
            var inject = func(new GenericParameter(TDependency));

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
        public virtual void ResolvedGeneric_NoName()
        {
            _ = new GenericParameter(null);
        }

        #endregion


        #region Implementation

        private void Assert_Generic_Injected(Type definition, Type type, InjectionMember member, string import, bool isNamed, object registered, object named)
        {
            Container.RegisterType(null, definition, null, null, member);
            
            var target = definition.MakeGenericType(type);

            // Validate
            Assert.ThrowsException<ResolutionFailedException>(() => Container.Resolve(target, null));

            // Register missing types
            RegisterTypes();

            // Act
            var instance = Container.Resolve(target, null) as PatternBaseType;
            var expected = !IMPORT_IMPLICIT.Equals(import) && isNamed ? named : registered;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }

        #endregion
    }
}
