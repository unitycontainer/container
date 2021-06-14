using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Constructors
{
    [TestClass]
    public partial class Resolving_Implicit : Dependency.Implicit.Pattern
    {
        #region Properties
        protected override string DependencyName => "value";

        #endregion


        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void Resolving_Implicit_Initialize(TestContext context) => Pattern_Initialize(context.FullyQualifiedTestClassName);

        #endregion
    }
}
