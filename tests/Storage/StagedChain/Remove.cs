using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Container.Tests;
using Unity.Extension;

namespace Storage
{
    public partial class StagedChainTests
    {
        [PatternTestMethod("Remove(key)"), TestProperty(TEST, REMOVE)]
        public void Remove()
        {
            Chain.Add(new [] 
            {
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,       Segment0),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,  Segment1),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation, Segment2),
            });

            Assert.IsTrue(Chain.Remove(UnityBuildStage.Setup));

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
            Chain.Add(new[]
            {
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,       Segment0),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,  Segment1),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation, Segment2),
            });

            Assert.IsTrue(Chain.Remove(UnityBuildStage.Setup));
            Assert.IsTrue(fired);
        }

        [PatternTestMethod("Remove(key) from empty"), TestProperty(TEST, REMOVE)]
        public void Remove_Empty()
        {
            Assert.IsFalse(Chain.Remove(UnityBuildStage.Setup));
        }

        [PatternTestMethod("Remove(key, value)"), TestProperty(TEST, REMOVE)]
        public void Remove_Pair()
        {
            Chain.Add(new[]
            {
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,       Segment0),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,  Segment1),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation, Segment2),
            });

            Assert.IsTrue(Chain.Remove(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,        Segment0)));
            Assert.IsFalse(Chain.Remove(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation, Segment0)));
            
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
            Chain.Add(new[]
            {
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,       Segment0),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,  Segment1),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation, Segment2),
            });

            Assert.IsTrue(Chain.Remove(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup, Segment0)));
            Assert.IsTrue(fired);
        }
    }
}
