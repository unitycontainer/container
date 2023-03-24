using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

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
        const string MAPPED       = "Mapped";

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

        public interface IService
        {
        }

        public class Service : IService
        {
        }

        public class SharedService : IService
        {
        }

        public class NonSharedService : IService
        {
        }

        #endregion
    }
}
