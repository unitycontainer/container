using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;

namespace Resolution
{
    [TestClass]
    public partial class Mapping : PatternBase
    {
        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void ClassInit(TestContext context) => PatternBaseInitialize(context.FullyQualifiedTestClassName);

        #endregion
    }
}
