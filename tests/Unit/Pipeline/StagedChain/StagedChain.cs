using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Unity.Builder;
using Unity.Storage;

namespace Pipeline
{
    public partial class StagedChain
    {
        [TestMethod, TestProperty(TEST, nameof(StagedStrategyChain<Unresolvable, UnityBuildStage>))]
        public void Empty()
        {
            Assert.IsFalse(Chain.IsReadOnly);
            Assert.AreEqual(0, Chain.Count);
            Assert.AreEqual(0, Chain.Keys.Count);
            Assert.AreEqual(0, Chain.Values.Count);
            Assert.AreEqual(0, Chain.Values.ToArray().Length);
        }

        [TestMethod, TestProperty(TEST, nameof(StagedStrategyChain<Unresolvable, UnityBuildStage>))]
        public void Keys()
        {
            Chain.Add((UnityBuildStage.Setup,        Segment0),
                      (UnityBuildStage.Diagnostic,   Segment1),
                      (UnityBuildStage.PreCreation,  Segment2),
                      (UnityBuildStage.Creation,     Segment3),
                      (UnityBuildStage.PostCreation, Segment4));

            Assert.AreEqual(5, Chain.Count);
            Assert.AreEqual(5, Chain.Keys.Count);
        }

        [TestMethod, TestProperty(TEST, nameof(StagedStrategyChain<Unresolvable, UnityBuildStage>))]
        public void Values()
        {
            Chain.Add((UnityBuildStage.Setup,        Segment0),
                      (UnityBuildStage.Diagnostic,   Segment1),
                      (UnityBuildStage.PreCreation,  Segment2),
                      (UnityBuildStage.Creation,     Segment3),
                      (UnityBuildStage.PostCreation, Segment4));

            Assert.AreEqual(5, Chain.Count);
            Assert.AreEqual(5, Chain.Values.Count);
        }

        [TestMethod, TestProperty(TEST, nameof(StagedStrategyChain<Unresolvable, UnityBuildStage>))]
        public void ToArray()
        {
            Chain.Add((UnityBuildStage.Setup,        Segment0),
                      (UnityBuildStage.Diagnostic,   Segment1),
                      (UnityBuildStage.PreCreation,  Segment2),
                      (UnityBuildStage.Creation,     Segment3),
                      (UnityBuildStage.PostCreation, Segment4));

            Assert.AreEqual(5, Chain.Count);
            Assert.AreEqual(5, Chain.Values.ToArray().Length);
        }

        [TestMethod, TestProperty(TEST, nameof(StagedStrategyChain<Unresolvable, UnityBuildStage>))]
        public void ForEach()
        {
            Chain.Add((UnityBuildStage.Setup,        Segment0),
                      (UnityBuildStage.Diagnostic,   Segment1),
                      (UnityBuildStage.Creation,     Segment3),
                      (UnityBuildStage.PostCreation, Segment4));
            
            var count = 0;
            foreach (var pair in Chain)
            {
                Assert.IsNotNull(pair);
                count++;
            }
            
            Assert.AreEqual(count, Chain.Count);
        }


        [TestMethod, TestProperty(TEST, nameof(StagedStrategyChain<Unresolvable, UnityBuildStage>))]
        public void Clear()
        {
            Chain.Add((UnityBuildStage.Setup,       Segment0),
                      (UnityBuildStage.Diagnostic,  Segment1),
                      (UnityBuildStage.PreCreation, Segment2),
                      (UnityBuildStage.Creation,    Segment3),
                      (UnityBuildStage.PostCreation, Segment4));
            
            Chain.Clear();
            Assert.AreEqual(0, Chain.Count);
            Assert.AreEqual(0, Chain.Keys.Count);
            Assert.AreEqual(0, Chain.Values.Count);
            Assert.AreEqual(0, Chain.Values.ToArray().Length);
        }
    }
}
