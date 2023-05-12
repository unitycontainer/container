using BenchmarkDotNet.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Storage;

namespace Container.Scope
{
    [TestClass]
    public class HashScopeTests : ScopeTests
    {
        #region Scaffolding

        #endregion

        [IterationSetup]
        [TestInitialize]
        public virtual void InitializeTest() => Scope = new HashScope(1);
    }
}
