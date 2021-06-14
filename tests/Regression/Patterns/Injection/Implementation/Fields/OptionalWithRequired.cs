using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fields
{
    [TestClass]
    public partial class Injecting_Optional_With_Required : Injection.Optional.Pattern
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
