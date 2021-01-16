using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Lifetime
{
    [TestClass]
    public partial class Managers
    {
        #region Constants

        protected const string LIFETIME = "Lifetime";
        protected const string BASE_TYPE = nameof(RegistrationManager);
        protected const string INJECTION = "Injection";
        protected const string POLICYSET = "PolicySet";

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
