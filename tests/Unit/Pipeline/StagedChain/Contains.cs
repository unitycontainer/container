using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Container.Tests;

namespace Pipeline
{
    public partial class StagedChain
    {
        [PatternTestMethod("Contains(key)"), TestProperty(TEST, CONTAINS)]
        public void Indexer_ContainsKey()
        {
            Chain.Add((UnityBuildStage.Setup, Segment0),
                      (UnityBuildStage.Diagnostic,  Segment1),
                      (UnityBuildStage.PreCreation,  Segment2));

            Assert.IsTrue(Chain.ContainsKey(UnityBuildStage.Setup));
            Assert.IsTrue(Chain.ContainsKey(UnityBuildStage.Diagnostic));
            Assert.IsTrue(Chain.ContainsKey(UnityBuildStage.PreCreation));
            Assert.IsFalse(Chain.ContainsKey(UnityBuildStage.Creation));
            Assert.IsFalse(Chain.ContainsKey(UnityBuildStage.PostCreation));
        }

        [PatternTestMethod("Contains(key, value)"), TestProperty(TEST, CONTAINS)]
        public void Indexer_ContainsKeyValue()
        {
            Chain.Add((UnityBuildStage.Setup, Segment0),
                      (UnityBuildStage.Diagnostic,  Segment1),
                      (UnityBuildStage.PreCreation,  Segment2));

            Assert.IsTrue(Chain.Contains(new KeyValuePair<UnityBuildStage, Unresolvable>(UnityBuildStage.Setup,      Segment0)));
            Assert.IsTrue(Chain.Contains(new KeyValuePair<UnityBuildStage, Unresolvable>(UnityBuildStage.Diagnostic,  Segment1)));
            Assert.IsTrue(Chain.Contains(new KeyValuePair<UnityBuildStage, Unresolvable>(UnityBuildStage.PreCreation,  Segment2)));

            Assert.IsFalse(Chain.Contains(new KeyValuePair<UnityBuildStage, Unresolvable>(UnityBuildStage.Setup,  Segment2)));
            Assert.IsFalse(Chain.Contains(new KeyValuePair<UnityBuildStage, Unresolvable>(UnityBuildStage.Creation, Segment3)));
            Assert.IsFalse(Chain.Contains(new KeyValuePair<UnityBuildStage, Unresolvable>(UnityBuildStage.PostCreation,  Segment4)));
        }
    }
}
