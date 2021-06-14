using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fields
{
    [TestClass]
    public class Selecting_Annotated : Selection.Annotated.Pattern
    {
        #region Properties
        protected override string DependencyName => "Field";

        #endregion


        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void Selecting_Annotated_Initialize(TestContext context) 
            => Pattern_Initialize(context.FullyQualifiedTestClassName);

        #endregion
    }
}
