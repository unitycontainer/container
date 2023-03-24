using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Storage;
using Unity.Strategies;

namespace Pipeline
{
    [TestClass]
    public partial class StagedChain
    {
        #region Constants

        const string TEST              = "Testing";
        const string ADD               = nameof(IDictionary.Add);
        const string INDEXER           = "Indexer";
        const string CONTAINS          = nameof(IDictionary.Contains);
        const string REMOVE            = nameof(IDictionary.Remove);
        const string TO_ARRAY          = "ToArray()";
        const string BUILDUP           = "BuildUp";
        const string BUILDUP_COMPILED  = "BuildUp Compiled";
        const string ANALYSIS          = "Analysis";
        const string ANALYSIS_COMPILED = "Analysis Compiled";


        #endregion


        #region Fields

        StagedStrategyChain<BuilderStrategy, UnityBuildStage> Chain;
        FakeContext Context;

        static readonly Unresolvable Segment0 = Unresolvable.Create("0");
        static readonly Unresolvable Segment1 = Unresolvable.Create("1");
        static readonly Unresolvable Segment2 = Unresolvable.Create("2");
        static readonly Unresolvable Segment3 = Unresolvable.Create("3");
        static readonly Unresolvable Segment4 = Unresolvable.Create("4");
        static readonly Unresolvable Segment5 = Unresolvable.Create("5");

        #endregion


        #region Scaffolding

        [TestInitialize]
        public void TestInitialize()
        {
            Chain = new StagedStrategyChain<BuilderStrategy, UnityBuildStage>();
            Context = new FakeContext()
            { 
                Existing = new List<string>()
            };
        }

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
