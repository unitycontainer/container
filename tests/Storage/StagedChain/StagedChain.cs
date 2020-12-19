using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Storage;

namespace Storage
{
    public partial class StagedChainTests
    {
        [TestMethod, TestProperty(TEST, nameof(StagedChain<TestEnum, Unresolvable>))]
        public void Empty()
        {
            Assert.IsFalse(Chain.IsReadOnly);
            Assert.AreEqual(0, Chain.Count);
            Assert.AreEqual(0, Chain.Keys.Count);
            Assert.AreEqual(0, Chain.Values.Count);
            Assert.AreEqual(0, Chain.ToArray().Length);
        }

        [TestMethod, TestProperty(TEST, nameof(StagedChain<TestEnum, Unresolvable>))]
        public void Keys()
        {
            Chain.Add(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One,  Segment1),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two,  Segment2),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Three, Segment3),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Four, Segment4));

            Assert.AreEqual(5, Chain.Count);
            Assert.AreEqual(5, Chain.Keys.Count);
        }

        [TestMethod, TestProperty(TEST, nameof(StagedChain<TestEnum, Unresolvable>))]
        public void Values()
        {
            Chain.Add(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One, Segment1),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two, Segment2),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Three, Segment3),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Four, Segment4));

            Assert.AreEqual(5, Chain.Count);
            Assert.AreEqual(5, Chain.Values.Count);
        }

        [TestMethod, TestProperty(TEST, nameof(StagedChain<TestEnum, Unresolvable>))]
        public void ToArray()
        {
            Chain.Add(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One, Segment1),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two, Segment2),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Three, Segment3),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Four, Segment4));

            Assert.AreEqual(5, Chain.Count);
            Assert.AreEqual(5, Chain.ToArray().Length);
        }

        [TestMethod, TestProperty(TEST, nameof(StagedChain<TestEnum, Unresolvable>))]
        public void ForEach()
        {
            Chain.Add(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One, Segment1),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Three, Segment3),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Four, Segment4));
            
            var count = 0;
            foreach (var pair in Chain)
            {
                Assert.IsNotNull(pair);
                count++;
            }
            
            Assert.AreEqual(count, Chain.Count);
        }


        [TestMethod, TestProperty(TEST, nameof(StagedChain<TestEnum, Unresolvable>))]
        public void Clear()
        {
            Chain.Add(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One, Segment1),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two, Segment2),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Three, Segment3),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Four, Segment4));
            
            Chain.Clear();
            Assert.AreEqual(0, Chain.Count);
            Assert.AreEqual(0, Chain.Keys.Count);
            Assert.AreEqual(0, Chain.Values.Count);
            Assert.AreEqual(0, Chain.ToArray().Length);
        }
    }
}
