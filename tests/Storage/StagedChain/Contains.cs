using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Container.Tests;

namespace Storage
{
    public partial class StagedChainTests
    {
        [PatternTestMethod("Contains(key)"), TestProperty(TEST, CONTAINS)]
        public void Indexer_ContainsKey()
        {
            Chain.Add(new [] 
            {
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One,  Segment1),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two,  Segment2),
            });

            Assert.IsTrue(Chain.ContainsKey(TestEnum.Zero));
            Assert.IsTrue(Chain.ContainsKey(TestEnum.One));
            Assert.IsTrue(Chain.ContainsKey(TestEnum.Two));
            Assert.IsFalse(Chain.ContainsKey(TestEnum.Three));
            Assert.IsFalse(Chain.ContainsKey(TestEnum.Four));
        }

        [PatternTestMethod("Contains(key, value)"), TestProperty(TEST, CONTAINS)]
        public void Indexer_ContainsKeyValue()
        {
            Chain.Add(new[]
            {
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One,  Segment1),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two,  Segment2),
            });

            Assert.IsTrue(Chain.Contains(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0)));
            Assert.IsTrue(Chain.Contains(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One,  Segment1)));
            Assert.IsTrue(Chain.Contains(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two,  Segment2)));

            Assert.IsFalse(Chain.Contains(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero,  Segment2)));
            Assert.IsFalse(Chain.Contains(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Three, Segment3)));
            Assert.IsFalse(Chain.Contains(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Four,  Segment4)));
        }
    }
}
