using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Container.Tests;

namespace Pipeline
{
    public partial class StagedChain
    {
        [PatternTestMethod("value = Empty[key]"), TestProperty(TEST, INDEXER)]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Indexer_Get_FromEmpty()
        {
            Assert.AreSame(Segment2, Chain[UnityBuildStage.PreCreation]);
        }

        [PatternTestMethod("value = Chain[key]"), TestProperty(TEST, INDEXER)]
        public void Indexer_Get()
        {
            Assert.AreEqual(0, Chain.Version);
            Chain.Add((UnityBuildStage.Setup,        Segment0),
                      (UnityBuildStage.Diagnostic,   Segment1),
                      (UnityBuildStage.PreCreation,  Segment2),
                      (UnityBuildStage.Creation,     Segment3),
                      (UnityBuildStage.PostCreation, Segment4));

            Assert.AreEqual(5, Chain.Count);
            Assert.AreEqual(1, Chain.Version);
            Assert.AreSame(Segment0, Chain[UnityBuildStage.Setup]);
            Assert.AreSame(Segment1, Chain[UnityBuildStage.Diagnostic]);
            Assert.AreSame(Segment2, Chain[UnityBuildStage.PreCreation]);
            Assert.AreSame(Segment3, Chain[UnityBuildStage.Creation]);
            Assert.AreSame(Segment4, Chain[UnityBuildStage.PostCreation]);
            Assert.AreEqual(1, Chain.Version);
        }

        [PatternTestMethod("Chain[key] = value"), TestProperty(TEST, INDEXER)]
        public void Indexer_Set()
        {
            Assert.AreEqual(0, Chain.Version);
            Chain[UnityBuildStage.Setup]        = Segment0;
            Chain[UnityBuildStage.Diagnostic]   = Segment1;
            Chain[UnityBuildStage.PreCreation]  = Segment2;
            Chain[UnityBuildStage.Creation]     = Segment3;
            Chain[UnityBuildStage.PostCreation] = Segment4;

            Assert.AreEqual(5, Chain.Version);
            Assert.AreEqual(5, Chain.Count);
            Assert.AreSame(Segment0, Chain[UnityBuildStage.Setup] );
            Assert.AreSame(Segment1, Chain[UnityBuildStage.Diagnostic]  );
            Assert.AreSame(Segment2, Chain[UnityBuildStage.PreCreation]  );
            Assert.AreSame(Segment3, Chain[UnityBuildStage.Creation]);
            Assert.AreSame(Segment4, Chain[UnityBuildStage.PostCreation] );
        }

        [TestMethod("Chain[key] = value Fires Event"), TestProperty(TEST, INDEXER)]
        public void Indexer_Set_FiresEvent()
        {
            var fired = false;

            Chain.Invalidated += (c, t) => fired = true;

            Assert.AreEqual(0, Chain.Count);
            Chain[UnityBuildStage.Setup] = Segment0;

            Assert.AreEqual(1, Chain.Version);
            Assert.AreEqual(1, Chain.Count);
            Assert.IsTrue(fired);
        }

        [PatternTestMethod("Chain.TryGetValue(...)"), TestProperty(TEST, INDEXER)]
        public void Indexer_TryGetValue()
        {
            Chain.Add((UnityBuildStage.Setup,        Segment0),
                      (UnityBuildStage.Diagnostic,   Segment1),
                      (UnityBuildStage.PreCreation,  Segment2),
                      (UnityBuildStage.Creation,     Segment3),
                      (UnityBuildStage.PostCreation, Segment4));

            Assert.AreEqual(1, Chain.Version);
            Assert.AreEqual(5, Chain.Count);
            Assert.IsTrue(Chain.TryGetValue(UnityBuildStage.PreCreation, out var value));
            Assert.AreSame(Segment2, value);
        }


        [PatternTestMethod("Empty.TryGetValue(...)"), TestProperty(TEST, INDEXER)]
        public void Indexer_TryGetValue_FromEmpty()
        {
            Assert.AreEqual(0, Chain.Count);
            Assert.IsFalse(Chain.TryGetValue(UnityBuildStage.PreCreation, out var value));
            Assert.IsNull(value);
        }
    }
}
