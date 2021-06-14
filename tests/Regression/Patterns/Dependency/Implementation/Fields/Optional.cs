using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;

namespace Fields
{
    [TestClass]
    public partial class Resolving_Optional : Dependency.Optional.Pattern
    {
        #region Properties

        protected override string DependencyName => "Field";

        #endregion


        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void Resolving_Optional_Initialize(TestContext context) 
            => Pattern_Initialize(context.FullyQualifiedTestClassName);

        #endregion
    }
}
