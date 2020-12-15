using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lifetime
{
    [TestClass]
    public partial class Managers
    {
        #region Constants

        protected const string LIFETIME = "Lifetime";

        #endregion


        [ClassInitialize]
        public static void InitializeClass(TestContext _)
        {
        }

        [TestInitialize]
        public virtual void InitializeTest()
        { 
        }
    }
}
