using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Storage;
using System.Collections;
using Unity.Extension;

namespace Storage
{
    [TestClass]
    public partial class StagedChainTests
    {
        #region Constants

        const string TEST = "Testing";
        const string ADD      = nameof(IDictionary.Add);
        const string INDEXER  = "Indexer";
        const string CONTAINS = nameof(IDictionary.Contains);
        const string REMOVE   = nameof(IDictionary.Remove);
        

        #endregion


        #region Fields

        StagedStrategyChain Chain;

        static readonly Unresolvable Segment0 = Unresolvable.Create("0");
        static readonly Unresolvable Segment1 = Unresolvable.Create("1");
        static readonly Unresolvable Segment2 = Unresolvable.Create("2");
        static readonly Unresolvable Segment3 = Unresolvable.Create("3");
        static readonly Unresolvable Segment4 = Unresolvable.Create("4");
        static readonly Unresolvable Segment5 = Unresolvable.Create("5");

        #endregion


        #region Scaffolding

        [TestInitialize]
        public void TestInitialize() => Chain = new StagedStrategyChain();

        #endregion


        #region Test Data

        public class Unresolvable : BuilderStrategy
        {
            public readonly string Id;

            protected Unresolvable(string id) { Id = id; }

            public static Unresolvable Create(string name) => new Unresolvable(name);

            public override string ToString() => $"Unresolvable.{Id}";
        }

        #endregion
    }
}
