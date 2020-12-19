using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Container.Tests;

namespace Storage
{
    public partial class StagedChainTests
    {

        [TestMethod("Add(...)"), TestProperty(TEST, ADD)]
        public void IDictionary_Add_TStageEnum_TStrategyType()
        {
            Chain.Add(TestEnum.Zero, Segment0);
            Assert.AreEqual(1, Chain.Count);

            Chain.Add(TestEnum.One, Segment1);
            Assert.AreEqual(2, Chain.Count);
        }

        [TestMethod("Add(...) throws if added twice"), TestProperty(TEST, ADD)]
        [ExpectedException(typeof(ArgumentException))]
        public void IDictionary_Add_ThrowsIfAddedTwice()
        {
            Chain.Add(TestEnum.Zero, Segment0);
            Chain.Add(TestEnum.Zero, Segment1);
        }

        [TestMethod("Add(...) Fires Event"), TestProperty(TEST, ADD)]
        public void IDictionary_Add_FiresEvent()
        {
            var fired = false;

            Chain.ChainChanged = (c) => fired = true;

            Chain.Add(TestEnum.Zero, Segment0);

            Assert.IsTrue(fired);
        }

        [PatternTestMethod("Add(KeyValuePair ...)"), TestProperty(TEST, ADD)]
        public void IDictionary_Add_KVP()
        {
            Chain.Add(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0));
            Assert.AreEqual(1, Chain.Count);

            Chain.Add(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One, Segment1));
            Assert.AreEqual(2, Chain.Count);
        }

        [PatternTestMethod("Add(KeyValuePair[] ...)"), TestProperty(TEST, ADD)]
        public void IDictionary_Add_KVP_Multiple()
        {
            Chain.Add(new [] 
            {
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One,  Segment1),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two,  Segment2),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Three,Segment3),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Four, Segment4),
            });
            Assert.AreEqual(5, Chain.Count);
        }

        [TestMethod("Add(KeyValuePair[] ...) throws if added twice"), TestProperty(TEST, ADD)]
        [ExpectedException(typeof(ArgumentException))]
        public void IDictionary_Add_KVP_ThrowsIfAddedTwice()
        {
            Chain.Add(new[]
            {
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One,  Segment1),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two,  Segment2),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Three,Segment3),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment4),
            });
        }


        [PatternTestMethod("Add(KeyValuePair[] ...) fires event once"), TestProperty(TEST, ADD)]
        public void IDictionary_Add_KVP_FiresOnce()
        {
            var count = 0;

            Chain.ChainChanged = (c) => count++;
            Chain.Add(new[]
            {
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One,  Segment1),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two,  Segment2),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Three,Segment3),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Four, Segment4),
            });
            
            Assert.AreEqual(1, count);
        }
    }
}
