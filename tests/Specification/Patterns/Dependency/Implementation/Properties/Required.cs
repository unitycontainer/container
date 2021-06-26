using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;

namespace Properties
{
    [TestClass]
    public partial class Resolving_Required : Dependency.Required.Pattern
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
