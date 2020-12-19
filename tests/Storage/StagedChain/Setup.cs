using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Storage;
using System.Collections;

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

        StagedChain<TestEnum, Unresolvable> Chain;

        static readonly Unresolvable Segment0 = Unresolvable.Create("0");
        static readonly Unresolvable Segment1 = Unresolvable.Create("1");
        static readonly Unresolvable Segment2 = Unresolvable.Create("2");
        static readonly Unresolvable Segment3 = Unresolvable.Create("3");
        static readonly Unresolvable Segment4 = Unresolvable.Create("4");
        static readonly Unresolvable Segment5 = Unresolvable.Create("5");

        #endregion


        #region Scaffolding

        [TestInitialize]
        public void TestInitialize() => Chain = new StagedChain<TestEnum, Unresolvable>();

        #endregion


        #region Test Data

        public enum TestEnum
        {
            Zero,
            One,
            Two,
            Three,
            Four
        }

        public enum TestIntEnum : int
        { 
            Zero, 
            One, 
            Two,
            Three,
            Four
        }

        public enum TestZeroedEnum
        {
            Zero = 0,
            One,
            Two,
            Three,
            Four
        }

        public class Unresolvable
        {
            public readonly string Id;

            protected Unresolvable(string id) { Id = id; }

            public static Unresolvable Create(string name) => new Unresolvable(name);

            public override string ToString() => $"Unresolvable.{Id}";
        }

        #endregion
    }

}
