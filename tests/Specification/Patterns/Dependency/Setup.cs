using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Resolution;
#endif

namespace Dependency
{
    public abstract partial class Pattern : PatternBase
    {
        #region Constants

        protected const string CATEGORY_DEPENDENCY = "Resolve";

        #endregion


        #region Properties

        protected static Type BaselineConsumer;
        protected static Type BaselineTestNamed;
        protected static Type BaselineTestType;

        protected Type CorrespondingTypeDefinition
            => Type.GetType($"{Category}.{Dependency}.{Member}.{TestContext.TestName}") ?? BaselineTestType;

        #endregion


        #region Scaffolding

        public static void Pattern_Initialize(string name, Assembly assembly = null)
        {
            PatternBaseInitialize(name);

            BaselineTestType = GetTestType("BaselineTestType`1");
            BaselineConsumer = GetTestType("BaselineConsumer`1");
            BaselineTestNamed = GetTestType("BaselineTestTypeNamed`1");
        }

        #endregion


        #region Runners

        protected virtual void Assert_Consumer(Type type, ResolverOverride @override, object value, object @default)
        {
            // Arrange
            var target = BaselineConsumer.MakeGenericType(type);
            Container.RegisterType(null, target, null, null);

            // Register missing types
            RegisterTypes();

            // Act
            var instance = Container.Resolve(target, null, @override) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(value, instance.Value);
            Assert.AreEqual(@default, instance.Default);
        }

        #endregion


        #region Test Data

        public static IEnumerable<object[]> WithDefaultValue_Data
        {
            get
            {
                var types = FromNamespace("WithDefault").ToArray();

                foreach (var type in types) yield return new object[] { type.Name, type };
                if (0 == types.Length) yield return new object[] { "Empty", typeof(DummyImport) };
            }
        }

        public static IEnumerable<object[]> WithDefaultAttribute_Data
        {
            get
            {
                var types = FromNamespace("WithDefaultAttribute").ToArray();

                foreach (var type in types) yield return new object[] { type.Name, type };
                if (0 == types.Length) yield return new object[] { "Empty", typeof(DummyImport) };
            }
        }

        public static IEnumerable<object[]> WithDefaultAndAttribute_Data
        {
            get
            {
                var types = FromNamespace("WithDefaultAndAttribute").ToArray();

                foreach (var type in types) yield return new object[] { type.Name, type };
                if (0 == types.Length) yield return new object[] { "Empty", typeof(DummyImport) };
            }
        }

        #endregion
    }
}
