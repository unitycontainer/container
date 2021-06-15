using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Reflection;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity.Resolution;
#endif

namespace Fields
{
    [TestClass]
    public partial class Injecting_Required_With_Optional : Injection.Required.Pattern
    {
        #region Properties

        protected override string DependencyName => "Field";

        #endregion


        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void Injecting_Required_With_Optional_Initialize(TestContext context) 
            => Injecting_Required_With_Optional_Initialize(context.FullyQualifiedTestClassName);

        public static void Injecting_Required_With_Optional_Initialize(string name, Assembly assembly = null)
        {
            Pattern_Initialize(name);

            Type support = Type.GetType($"{typeof(PatternBase).FullName}+{Member}");

            if (support is null) return;

            // Override injectors with optional
            InjectionMember_Value = (Func<object, InjectionMember>)support
                .GetMethod("GetInjectionValueOptional").CreateDelegate(typeof(Func<object, InjectionMember>));

            InjectionMember_Default = (Func<InjectionMember>)support
                .GetMethod("GetInjectionDefaultOptional").CreateDelegate(typeof(Func<InjectionMember>));

            InjectionMember_Contract = (Func<Type, string, InjectionMember>)support
                .GetMethod("GetInjectionContractOptional").CreateDelegate(typeof(Func<Type, string, InjectionMember>));
        }

        #endregion


        #region Overrides

        protected override void Assert_Injection(Type type, InjectionMember member, object @default, object expected)
        {
            // Inject
            Container.RegisterType(null, type, null, null, member);

            // Act
            var instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(@default, instance.Value);

            // Register missing types
            RegisterTypes();

            // Act
            instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }

#if BEHAVIOR_V4
        public override void Inject_WithType(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }
#endif
        #endregion
    }
}
