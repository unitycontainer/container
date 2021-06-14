using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Constructors
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
        public static void Injecting_Implicit_Initialize(TestContext context) 
            => Pattern_Initialize(context.FullyQualifiedTestClassName);

        #endregion


        #region Not Supported

#if !UNITY_V4
        public override void Inject_Default(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }
        public override void Inject_Named_Default(string test, Type type, object defaultValue, object defaultAttr, object registered, object named, object injected, object overridden, object @default) { }
#endif
        #endregion
    }
}
