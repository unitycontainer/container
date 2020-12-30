using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition;
using Unity;
using Unity.Lifetime;

namespace Container
{
    [TestClass]
    public partial class Resolution
    {
        #region Constants

        const string Balanced  = "balanced";
        const string Singleton = "singleton";
        const string Optimized = "optimized";

        const string RESOLVE      = "Resolving";
        const string REGISTERED   = "Registered";
        const string UNREGISTERED = "Unregistered";

        #endregion


        #region Scaffolding

        protected IUnityContainer Container;

        [TestInitialize]
        public void TestInitialize()
        {
            Container = new UnityContainer();
        }

        #endregion


        #region Test Data

        public class Service
        {
        }

        [PartCreationPolicy(CreationPolicy.Shared)]
        public class SharedService
        {
        }

        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class NonSharedService
        {
        }

        #endregion
    }
}
