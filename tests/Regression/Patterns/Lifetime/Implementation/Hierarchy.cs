using Microsoft.VisualStudio.TestTools.UnitTesting;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Lifetime
{
    [TestClass]
    public class Hierarchy : Lifetime.Hierarchies.Pattern
    {
        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        #endregion


        #region Disabled Tests
        
        
        
        
        #endregion
    }
}
