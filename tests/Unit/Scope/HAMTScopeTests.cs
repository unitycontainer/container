using BenchmarkDotNet.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Storage;

namespace Container.Scope
{
   // [TestClass]
    public class HAMTScopeTests : ScopeTests
    {
        [IterationSetup]
        [TestInitialize]
        public virtual void InitializeTest() => Scope = new HAMTScope(1);
    }
}
