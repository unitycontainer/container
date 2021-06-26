using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Properties
{
    [TestClass]
    public partial class Injecting_Required_With_Required : Injection.Required.Pattern
    {
        #region Properties

        protected override string DependencyName => "Property";

        #endregion


        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void ClassInit(TestContext context) => Pattern_Initialize(context.FullyQualifiedTestClassName);

        #endregion
    }
}
