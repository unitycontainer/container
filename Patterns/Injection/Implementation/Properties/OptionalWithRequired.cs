using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Properties
{
    [TestClass]
    public partial class Injecting_Optional_With_Required : Injection.Optional.Pattern
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
