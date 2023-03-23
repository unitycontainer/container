using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Container.Tests;
using Unity.Strategies;

namespace Pipeline
{
    public partial class StagedChain
    {
        [PatternTestMethod("Remove(key)"), TestProperty(TEST, REMOVE)]
        public void Remove()
        {
            Chain.Add((UnityBuildStage.Setup,       Segment0),
                      (UnityBuildStage.Diagnostic,  Segment1),
                      (UnityBuildStage.PreCreation, Segment2));

            Assert.AreEqual(1, Chain.Version);
            Assert.AreEqual(3, Chain.Count);
            Assert.IsTrue(Chain.Remove(UnityBuildStage.Setup));

            Assert.AreEqual(2, Chain.Version);
            Assert.AreEqual(2, Chain.Count);
            Assert.IsFalse(Chain.ContainsKey(UnityBuildStage.Setup));
            Assert.IsTrue(Chain.ContainsKey(UnityBuildStage.Diagnostic));
            Assert.IsTrue(Chain.ContainsKey(UnityBuildStage.PreCreation));
            Assert.IsFalse(Chain.ContainsKey(UnityBuildStage.Creation));
            Assert.IsFalse(Chain.ContainsKey(UnityBuildStage.PostCreation));
        }

        [PatternTestMethod("Remove(key) Fires Event"), TestProperty(TEST, REMOVE)]
        public void Remove_FiresEvent()
        {
            var fired = false;

            Chain.Invalidated += (c, t) => fired = true;
            Assert.AreEqual(0, Chain.Count);
            Chain.Add((UnityBuildStage.Setup,       Segment0),
                      (UnityBuildStage.Diagnostic,  Segment1),
                      (UnityBuildStage.PreCreation, Segment2));

            Assert.AreEqual(1, Chain.Version);
            Assert.AreEqual(3, Chain.Count);
            Assert.IsTrue(Chain.Remove(UnityBuildStage.Setup));
            Assert.AreEqual(2, Chain.Count);
            Assert.AreEqual(2, Chain.Version);
            Assert.IsTrue(fired);
        }

        [PatternTestMethod("Remove(key) from empty"), TestProperty(TEST, REMOVE)]
        public void Remove_Empty()
        {
            Assert.AreEqual(0, Chain.Count);
            Assert.IsFalse(Chain.Remove(UnityBuildStage.Setup));
            Assert.AreEqual(0, Chain.Count);
        }

        [PatternTestMethod("Remove(key, value)"), TestProperty(TEST, REMOVE)]
        public void Remove_Pair()
        {
            Chain.Add((UnityBuildStage.Setup,       Segment0),
                      (UnityBuildStage.Diagnostic,  Segment1),
                      (UnityBuildStage.PreCreation, Segment2));

            Assert.AreEqual(1, Chain.Version);
            Assert.AreEqual(3, Chain.Count);
            Assert.IsTrue(Chain.Remove(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,        Segment0)));
            Assert.IsFalse(Chain.Remove(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation, Segment0)));

            Assert.AreEqual(2, Chain.Version);
            Assert.AreEqual(2, Chain.Count);
            Assert.IsFalse(Chain.ContainsKey(UnityBuildStage.Setup));
            Assert.IsTrue(Chain.ContainsKey(UnityBuildStage.Diagnostic));
            Assert.IsTrue(Chain.ContainsKey(UnityBuildStage.PreCreation));
            Assert.IsFalse(Chain.ContainsKey(UnityBuildStage.Creation));
            Assert.IsFalse(Chain.ContainsKey(UnityBuildStage.PostCreation));
        }

        [PatternTestMethod("Remove(key, value) Fires Event"), TestProperty(TEST, REMOVE)]
        public void Remove_Pair_FiresEvent()
        {
            var fired = false;

            Chain.Invalidated += (c, t) => fired = true;
            Chain.Add((UnityBuildStage.Setup,       Segment0),
                      (UnityBuildStage.Diagnostic,  Segment1),
                      (UnityBuildStage.PreCreation, Segment2));

            Assert.AreEqual(1, Chain.Version);
            Assert.AreEqual(3, Chain.Count);
            Assert.IsTrue(Chain.Remove(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup, Segment0)));
            Assert.IsTrue(fired);
            Assert.AreEqual(2, Chain.Version);
            Assert.AreEqual(2, Chain.Count);
        }
    }
}
