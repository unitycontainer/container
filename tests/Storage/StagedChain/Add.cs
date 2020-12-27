using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Container.Tests;
using Unity.Extension;

namespace Storage
{
    public partial class StagedChainTests
    {
        #region Add

        [TestMethod("Add(...)"), TestProperty(TEST, ADD)]
        public void IDictionary_Add_TStageEnum_TStrategyType()
        {
            Chain.Add(UnityBuildStage.Setup, Segment0);
            Assert.AreEqual(1, Chain.Count);

            Chain.Add(UnityBuildStage.Diagnostic, Segment1);
            Assert.AreEqual(2, Chain.Count);
        }

        [TestMethod("Add(...) throws if added twice"), TestProperty(TEST, ADD)]
        [ExpectedException(typeof(ArgumentException))]
        public void IDictionary_Add_ThrowsIfAddedTwice()
        {
            Chain.Add(UnityBuildStage.Setup, Segment0);
            Chain.Add(UnityBuildStage.Setup, Segment1);
        }

        [TestMethod("Add(...) Fires Event"), TestProperty(TEST, ADD)]
        public void IDictionary_Add_FiresEvent()
        {
            var fired = false;

            ((IStagedStrategyChain)Chain).Invalidated += (c, t) => fired = true;

            Chain.Add(UnityBuildStage.Setup, Segment0);

            Assert.IsTrue(fired);
        }

        #endregion

        #region Key/Value Pair

        [PatternTestMethod("Add(KeyValuePair ...)"), TestProperty(TEST, ADD)]
        public void IDictionary_Add_KVP()
        {
            Chain.Add(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup, Segment0));
            Assert.AreEqual(1, Chain.Count);

            Chain.Add(new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic, Segment1));
            Assert.AreEqual(2, Chain.Count);
        }

        [PatternTestMethod("Add(KeyValuePair[] ...)"), TestProperty(TEST, ADD)]
        public void IDictionary_Add_KVP_Multiple()
        {
            Chain.Add(new [] 
            {
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,        Segment0),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,   Segment1),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation,  Segment2),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Creation,     Segment3),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PostCreation, Segment4),
            });
            Assert.AreEqual(5, Chain.Count);
        }

        [TestMethod("Add(KeyValuePair[] ...) throws if added twice"), TestProperty(TEST, ADD)]
        [ExpectedException(typeof(ArgumentException))]
        public void IDictionary_Add_KVP_ThrowsIfAddedTwice()
        {
            Chain.Add(new[]
            {
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,       Segment0),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,  Segment1),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation, Segment2),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Creation,    Segment3),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,       Segment4),
            });
        }

        [PatternTestMethod("Add(KeyValuePair[] ...) fires event once"), TestProperty(TEST, ADD)]
        public void IDictionary_Add_KVP_FiresOnce()
        {
            var count = 0;

            ((IStagedStrategyChain)Chain).Invalidated += (c, t) => count++;
            Chain.Add(new[]
            {
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Setup,        Segment0),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Diagnostic,   Segment1),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PreCreation,  Segment2),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.Creation,     Segment3),
                new KeyValuePair<UnityBuildStage, BuilderStrategy>(UnityBuildStage.PostCreation, Segment4),
            });
            
            Assert.AreEqual(1, count);
        }

        #endregion


        #region ValueTuple


        [PatternTestMethod("Add(ValueTuple ...)"), TestProperty(TEST, ADD)]
        public void IDictionary_Add_ValueTuple()
        {
            Chain.Add((UnityBuildStage.Setup, Segment0));
            Assert.AreEqual(1, Chain.Count);

            Chain.Add((UnityBuildStage.Diagnostic, Segment1));
            Assert.AreEqual(2, Chain.Count);
        }

        [PatternTestMethod("Add(ValueTuple[] ...)"), TestProperty(TEST, ADD)]
        public void IDictionary_Add_ValueTuple_Multiple()
        {
            Chain.Add(new (UnityBuildStage, BuilderStrategy)[]
            {
                (UnityBuildStage.Setup,        Segment0),
                (UnityBuildStage.Diagnostic,   Segment1),
                (UnityBuildStage.PreCreation,  Segment2),
                (UnityBuildStage.Creation,     Segment3),
                (UnityBuildStage.PostCreation, Segment4),
            });
            Assert.AreEqual(5, Chain.Count);
        }

        [TestMethod("Add(ValueTuple[] ...) throws if added twice"), TestProperty(TEST, ADD)]
        [ExpectedException(typeof(ArgumentException))]
        public void IDictionary_Add_ValueTuple_ThrowsIfAddedTwice()
        {
            Chain.Add((UnityBuildStage.Setup,       Segment0),
                      (UnityBuildStage.Diagnostic,  Segment1),
                      (UnityBuildStage.PreCreation, Segment2),
                      (UnityBuildStage.Creation,    Segment3),
                      (UnityBuildStage.Setup,       Segment4));
        }

        [PatternTestMethod("Add(ValueTuple[] ...) fires event once"), TestProperty(TEST, ADD)]
        public void IDictionary_Add_ValueTuple_FiresOnce()
        {
            var count = 0;

            ((IStagedStrategyChain)Chain).Invalidated += (c, t) => count++;
            Chain.Add(new (UnityBuildStage, BuilderStrategy)[]
            {
                (UnityBuildStage.Setup,        Segment0),
                (UnityBuildStage.Diagnostic,   Segment1),
                (UnityBuildStage.PreCreation,  Segment2),
                (UnityBuildStage.Creation,     Segment3),
                (UnityBuildStage.PostCreation, Segment4),
            });

            Assert.AreEqual(1, count);
        }

        #endregion
    }
}


