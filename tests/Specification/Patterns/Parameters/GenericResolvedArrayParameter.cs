using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections;
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

        [PatternTestMethod("GenericResolvedArrayParameter(T) with no arguments"), TestProperty(PARAMETER, nameof(GenericResolvedArrayParameter))]
        [DynamicData(nameof(Array_Parameters_Data))]
        public void GenericResolvedArrayParameter(Type type, Type definition,
                                                string member, string import,
                                                Func<object, InjectionMember> func,
                                                object registered, object named, object injected, object @default,
                                                bool isNamed)
            => Assert_GenericArray_Injected(definition, type,
                func(new GenericResolvedArrayParameter(TDependency)),
                Array.CreateInstance(type, 0));


        [PatternTestMethod("GenericResolvedArrayParameter(T, ...) with values"), TestProperty(PARAMETER, nameof(GenericResolvedArrayParameter))]
        [DynamicData(nameof(Array_Parameters_Data))]
        public void GenericResolvedArrayParameter_Value(Type type, Type definition,
                                                 string member, string import,
                                                 Func<object, InjectionMember> func,
                                                 object registered, object named, object injected, object @default,
                                                 bool isNamed)
            => Assert_GenericArray_Injected(definition, type,
                func(new GenericResolvedArrayParameter(TDependency, 
                    registered, 
                    named, 
                    injected
#if !BEHAVIOR_V4
                    ,@default
#endif
                    )),
                new object[] { 
                    registered, 
                    named, 
                    injected 
#if !BEHAVIOR_V4
                    ,@default
#endif
                });


        [PatternTestMethod("GenericResolvedArrayParameter(T, ...) with resolvers"), TestProperty(PARAMETER, nameof(GenericResolvedArrayParameter))]
        [DynamicData(nameof(Array_Parameters_Data))]
        public void GenericResolvedArrayParameter_Resolvers(Type type, Type definition,
                                                     string member, string import,
                                                     Func<object, InjectionMember> func,
                                                     object registered, object named, object injected, object @default,
                                                     bool isNamed)
        {
            Container.RegisterInstance(type, registered);
            Container.RegisterInstance(type, Name, named);

            Assert_GenericArray_Injected(definition, type,
                           func(new GenericResolvedArrayParameter(TDependency,
                                                           new ResolvedParameter(type),
                                                           new OptionalParameter(type, Name),
                                                           new InjectionParameter(injected),
                                                           new ValidatingResolver(@default))),
                           new object[] { registered, named, injected, @default });
        }

#if !BEHAVIOR_V4
        [PatternTestMethod("GenericResolvedArrayParameter(T, values) on object[] array"), TestProperty(PARAMETER, nameof(GenericResolvedArrayParameter))]
        [DynamicData(nameof(Array_Parameters_Data))]
        public void GenericResolvedArrayParameter_Object(Type type, Type definition,
                                                  string member, string import,
                                                  Func<object, InjectionMember> func,
                                                  object registered, object named, object injected, object @default,
                                                  bool isNamed)
        {
            var target = definition.MakeGenericType(type);
            var injection = func(new GenericResolvedArrayParameter(TDependency, registered, named, injected, @default));

            // Arrange
            Container.RegisterType(null, target, null, null, injection);

            // Act
            var instance = Container.Resolve(target, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);
            Assert.IsInstanceOfType(instance.Value, type.MakeArrayType());

            var list = instance.Value as IList;
            var expected = new object[] { registered, named, injected, @default };

            Assert.IsNotNull(list);
            Assert.IsNotNull(expected);

            for (var i = 0; i < expected.Length; i++)
                Assert.IsTrue(list.Contains(expected[i]));
        }
#endif

        #endregion


        #region Failing

        [PatternTestMethod("GenericResolvedArrayParameter(T) on not an array type"), TestProperty(PARAMETER, nameof(GenericResolvedArrayParameter))]
        [DynamicData(nameof(Parameters_Test_Data))]
#if BEHAVIOR_V4
        [ExpectedException(typeof(InvalidOperationException))]
#else
        [ExpectedException(typeof(ResolutionFailedException))]
#endif
        public void GenericResolvedArrayParameter_NotArray(Type type, Type definition,
                                                           string member, string import,
                                                           Func<object, InjectionMember> func,
                                                           object registered, object named, object injected, object @default,
                                                           bool isNamed)
            => Assert_ResolvedArray_Injected(definition, type,
                func(new GenericResolvedArrayParameter(TDependency)),
                Array.CreateInstance(type, 0));


        [PatternTestMethod("GenericResolvedArrayParameter(wrong) throws if no match"), TestProperty(PARAMETER, nameof(GenericResolvedArrayParameter))]
        [DynamicData(nameof(Array_Parameters_Data))]
#if BEHAVIOR_V4
        [ExpectedException(typeof(InvalidOperationException))]
#else
        [ExpectedException(typeof(ResolutionFailedException))]
#endif
        public void GenericResolvedArrayParameter_NoMatch(Type type, Type definition,
                                                          string member, string import,
                                                          Func<object, InjectionMember> func,
                                                          object registered, object named, object injected, object @default,
                                                          bool isNamed)
            => Assert_GenericArray_Injected(definition, type,
                func(new GenericResolvedArrayParameter("wrong")),
                Array.CreateInstance(type, 0));

        #endregion


        #region Implementation


        protected void Assert_GenericArray_Injected(Type definition, Type type, InjectionMember injection, object values)
        {
            var array = type.MakeArrayType();
            var target = definition.MakeGenericType(type);

            // Arrange
            Container.RegisterType(null, definition, null, null, injection);

            // Act
            var instance = Container.Resolve(target, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);
            Assert.IsInstanceOfType(instance.Value, array);

            var list = instance.Value as IList;
            var expected = values as IList;

            Assert.IsNotNull(list);
            Assert.IsNotNull(expected);

            for (var i = 0; i < expected.Count; i++)
                Assert.IsTrue(list.Contains(expected[i]));
        }

        #endregion
    }
}
