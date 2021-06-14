using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Constructors
{
    [TestClass]
    public class Selecting_Implicit : Selection.Implicit.Pattern
    {
        #region Properties

        protected override string DependencyName => "value";

        #endregion


        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void Selecting_Implicit_Initialize(TestContext context) 
            => Pattern_Initialize(context.FullyQualifiedTestClassName);

        #endregion
    }
}
