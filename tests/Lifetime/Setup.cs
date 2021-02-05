using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Lifetime;

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

        protected const string INJECTING   = "Injecting";
        protected const string CTOR        = "Ctor";
        protected const string SEQUENCE    = "Array";
        protected const string PARAMS      = "Params";

        #endregion



        [TestMethod("Baseline"), TestProperty(BASE_TYPE, INJECTION)]
        public void Injection_Baseline()
        {
            // Arrange
            var manager = new TransientLifetimeManager();

            // Validate
            Assert.IsNull(manager.Constructors);
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNull(manager.Methods);
            Assert.IsNull(manager.Other);
        }
    }
}
