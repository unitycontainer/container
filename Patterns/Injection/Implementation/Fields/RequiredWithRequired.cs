using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity;
#endif


namespace Fields
{
    [TestClass]
    public partial class Injecting_Required_With_Required : Injection.Required.Pattern
    {
        #region Properties

        protected override string DependencyName => "Field";

        #endregion


        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void Injecting_Required_With_Required_Initialize(TestContext context) 
            => Pattern_Initialize(context.FullyQualifiedTestClassName);

        #endregion


        #region Overrides

        protected override void Assert_Injection(Type type, InjectionMember member, object @default, object expected)
        {
            // Inject
            Container.RegisterType(null, type, null, null, member);

            // Act
            Assert.ThrowsException<ResolutionFailedException>(() => Container.Resolve(type, null));

            // Register missing types
            RegisterTypes();

            // Act
            var instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(expected, instance.Value);
        }

        #endregion
    }
}
