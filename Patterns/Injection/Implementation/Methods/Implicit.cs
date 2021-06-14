using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Methods
{
    [TestClass]
    public partial class Injecting_Implicit : Injection.Implicit.Pattern
    {
        #region Properties
        protected override string DependencyName => "value";

        #endregion


        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void ClassInit(TestContext context) => Pattern_Initialize(context.FullyQualifiedTestClassName);

        #endregion


        #region Special Cases

#if BEHAVIOR_V4
        // This only worked in constructor
        public override void Member_Injected_ByInjectionParameter(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }
        public override void Member_Injected_ByParameterRecursive(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }
        public override void Member_Injected_ByValue(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }
#endif

        #endregion
    }
}
