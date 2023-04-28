using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Storage;

namespace Container.Scope
{
    [TestClass]
    public class HAMTScopeTests : ScopeTests
    {
        #region Scaffolding

        [ClassInitialize]
        public static new void InitializeClass(TestContext context) 
            => ScopeTests.InitializeClass(context);

        #endregion

        [TestInitialize]
        public virtual void InitializeTest() => Scope = new HAMTScope(1);
    }
}
