using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Container.Tests;
using Unity.Extension;

namespace Storage
{
    public partial class StagedChainTests
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
            Chain.Add(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,        Segment0),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,   Segment1),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation,  Segment2),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Creation,     Segment3),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PostCreation, Segment4));

            Assert.AreSame(Segment0, Chain[UnityBuildStage.Setup]);
            Assert.AreSame(Segment1, Chain[UnityBuildStage.Diagnostic]);
            Assert.AreSame(Segment2, Chain[UnityBuildStage.PreCreation]);
            Assert.AreSame(Segment3, Chain[UnityBuildStage.Creation]);
            Assert.AreSame(Segment4, Chain[UnityBuildStage.PostCreation]);
        }

        [PatternTestMethod("Chain[key] = value"), TestProperty(TEST, INDEXER)]
        public void Indexer_Set()
        {
            Chain[UnityBuildStage.Setup]        = Segment0;
            Chain[UnityBuildStage.Diagnostic]   = Segment1;
            Chain[UnityBuildStage.PreCreation]  = Segment2;
            Chain[UnityBuildStage.Creation]     = Segment3;
            Chain[UnityBuildStage.PostCreation] = Segment4;

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

            Chain[UnityBuildStage.Setup] = Segment0;

            Assert.IsTrue(fired);
        }

        [PatternTestMethod("Chain.TryGetValue(...)"), TestProperty(TEST, INDEXER)]
        public void Indexer_TryGetValue()
        {
            Chain.Add(new[]
            {
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,        Segment0),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,   Segment1),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation,  Segment2),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Creation,     Segment3),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PostCreation, Segment4),
            });

            Assert.IsTrue(Chain.TryGetValue(UnityBuildStage.PreCreation, out var value));
            Assert.AreSame(Segment2, value);
        }


        [PatternTestMethod("Empty.TryGetValue(...)"), TestProperty(TEST, INDEXER)]
        public void Indexer_TryGetValue_FromEmpty()
        {
            Assert.IsFalse(Chain.TryGetValue(UnityBuildStage.PreCreation, out var value));
            Assert.IsNull(value);
        }
    }
}
