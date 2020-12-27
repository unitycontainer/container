using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Extension;
using Unity.Storage;

namespace Storage
{
    public partial class StagedChainTests
    {
        [TestMethod, TestProperty(TEST, nameof(StagedStrategyChain))]
        public void Empty()
        {
            Assert.IsFalse(Chain.IsReadOnly);
            Assert.AreEqual(0, Chain.Count);
            Assert.AreEqual(0, Chain.Keys.Count);
            Assert.AreEqual(0, Chain.Values.Count);
            Assert.AreEqual(0, Chain.ToArray().Length);
        }

        [TestMethod, TestProperty(TEST, nameof(StagedStrategyChain))]
        public void Keys()
        {
            Chain.Add(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,        Segment0),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,   Segment1),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation,  Segment2),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Creation,     Segment3),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PostCreation, Segment4));

            Assert.AreEqual(5, Chain.Count);
            Assert.AreEqual(5, Chain.Keys.Count);
        }

        [TestMethod, TestProperty(TEST, nameof(StagedStrategyChain))]
        public void Values()
        {
            Chain.Add(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,        Segment0),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,   Segment1),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation,  Segment2),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Creation,     Segment3),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PostCreation, Segment4));

            Assert.AreEqual(5, Chain.Count);
            Assert.AreEqual(5, Chain.Values.Count);
        }

        [TestMethod, TestProperty(TEST, nameof(StagedStrategyChain))]
        public void ToArray()
        {
            Chain.Add(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,        Segment0),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,   Segment1),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation,  Segment2),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Creation,     Segment3),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PostCreation, Segment4));

            Assert.AreEqual(5, Chain.Count);
            Assert.AreEqual(5, Chain.ToArray().Length);
        }

        [TestMethod, TestProperty(TEST, nameof(StagedStrategyChain))]
        public void ForEach()
        {
            Chain.Add(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,        Segment0),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,   Segment1),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Creation,     Segment3),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PostCreation, Segment4));
            
            var count = 0;
            foreach (var pair in Chain)
            {
                Assert.IsNotNull(pair);
                count++;
            }
            
            Assert.AreEqual(count, Chain.Count);
        }


        [TestMethod, TestProperty(TEST, nameof(StagedStrategyChain))]
        public void Clear()
        {
            Chain.Add(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,       Segment0),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,  Segment1),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation, Segment2),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Creation,    Segment3),
                      new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PostCreation, Segment4));
            
            Chain.Clear();
            Assert.AreEqual(0, Chain.Count);
            Assert.AreEqual(0, Chain.Keys.Count);
            Assert.AreEqual(0, Chain.Values.Count);
            Assert.AreEqual(0, Chain.ToArray().Length);
        }
    }
}
