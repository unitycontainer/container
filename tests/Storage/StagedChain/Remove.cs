using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Container.Tests;

namespace Storage
{
    public partial class StagedChainTests
    {
        [PatternTestMethod("Remove(key)"), TestProperty(TEST, REMOVE)]
        public void Remove()
        {
            Chain.Add(new [] 
            {
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One,  Segment1),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two,  Segment2),
            });

            Assert.IsTrue(Chain.Remove(TestEnum.Zero));

            Assert.IsFalse(Chain.ContainsKey(TestEnum.Zero));
            Assert.IsTrue(Chain.ContainsKey(TestEnum.One));
            Assert.IsTrue(Chain.ContainsKey(TestEnum.Two));
            Assert.IsFalse(Chain.ContainsKey(TestEnum.Three));
            Assert.IsFalse(Chain.ContainsKey(TestEnum.Four));
        }

        [PatternTestMethod("Remove(key) Fires Event"), TestProperty(TEST, REMOVE)]
        public void Remove_FiresEvent()
        {
            var fired = false;

            Chain.ChainChanged = (c) => fired = true;
            Chain.Add(new[]
            {
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One,  Segment1),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two,  Segment2),
            });

            Assert.IsTrue(Chain.Remove(TestEnum.Zero));
            Assert.IsTrue(fired);
        }

        [PatternTestMethod("Remove(key) from empty"), TestProperty(TEST, REMOVE)]
        public void Remove_Empty()
        {
            Assert.IsFalse(Chain.Remove(TestEnum.Zero));
        }

        [PatternTestMethod("Remove(key, value)"), TestProperty(TEST, REMOVE)]
        public void Remove_Pair()
        {
            Chain.Add(new[]
            {
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One,  Segment1),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two,  Segment2),
            });

            Assert.IsTrue(Chain.Remove(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0)));
            Assert.IsFalse(Chain.Remove(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two, Segment0)));
            
            Assert.IsFalse(Chain.ContainsKey(TestEnum.Zero));
            Assert.IsTrue(Chain.ContainsKey(TestEnum.One));
            Assert.IsTrue(Chain.ContainsKey(TestEnum.Two));
            Assert.IsFalse(Chain.ContainsKey(TestEnum.Three));
            Assert.IsFalse(Chain.ContainsKey(TestEnum.Four));
        }

        [PatternTestMethod("Remove(key, value) Fires Event"), TestProperty(TEST, REMOVE)]
        public void Remove_Pair_FiresEvent()
        {
            var fired = false;

            Chain.ChainChanged = (c) => fired = true;
            Chain.Add(new[]
            {
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One,  Segment1),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two,  Segment2),
            });

            Assert.IsTrue(Chain.Remove(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0)));
            Assert.IsTrue(fired);
        }
    }
}
