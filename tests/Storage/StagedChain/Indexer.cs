using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Container.Tests;

namespace Storage
{
    public partial class StagedChainTests
    {
        [PatternTestMethod("value = Empty[key]"), TestProperty(TEST, INDEXER)]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Indexer_Get_FromEmpty()
        {
            Assert.AreSame(Segment2, Chain[TestEnum.Two]);
        }

        [PatternTestMethod("value = Chain[key]"), TestProperty(TEST, INDEXER)]
        public void Indexer_Get()
        {
            Chain.Add(new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One,  Segment1),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two,  Segment2),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Three,Segment3),
                      new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Four, Segment4));

            Assert.AreSame(Segment0, Chain[TestEnum.Zero]);
            Assert.AreSame(Segment1, Chain[TestEnum.One]);
            Assert.AreSame(Segment2, Chain[TestEnum.Two]);
            Assert.AreSame(Segment3, Chain[TestEnum.Three]);
            Assert.AreSame(Segment4, Chain[TestEnum.Four]);
        }

        [PatternTestMethod("Chain[key] = value"), TestProperty(TEST, INDEXER)]
        public void Indexer_Set()
        {
            Chain[TestEnum.Zero]  = Segment0;
            Chain[TestEnum.One]   = Segment1;
            Chain[TestEnum.Two]   = Segment2;
            Chain[TestEnum.Three] = Segment3;
            Chain[TestEnum.Four]  = Segment4;

            Assert.AreSame(Segment0, Chain[TestEnum.Zero] );
            Assert.AreSame(Segment1, Chain[TestEnum.One]  );
            Assert.AreSame(Segment2, Chain[TestEnum.Two]  );
            Assert.AreSame(Segment3, Chain[TestEnum.Three]);
            Assert.AreSame(Segment4, Chain[TestEnum.Four] );
        }

        [TestMethod("Chain[key] = value Fires Event"), TestProperty(TEST, INDEXER)]
        public void Indexer_Set_FiresEvent()
        {
            var fired = false;

            Chain.ChainChanged = (c) => fired = true;

            Chain[TestEnum.Zero] = Segment0;

            Assert.IsTrue(fired);
        }

        [PatternTestMethod("Chain.TryGetValue(...)"), TestProperty(TEST, INDEXER)]
        public void Indexer_TryGetValue()
        {
            Chain.Add(new[]
            {
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Zero, Segment0),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.One,  Segment1),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Two,  Segment2),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Three,Segment3),
                new KeyValuePair<TestEnum, Unresolvable>(TestEnum.Four, Segment4),
            });

            Assert.IsTrue(Chain.TryGetValue(TestEnum.Two, out var value));
            Assert.AreSame(Segment2, value);
        }


        [PatternTestMethod("Empty.TryGetValue(...)"), TestProperty(TEST, INDEXER)]
        public void Indexer_TryGetValue_FromEmpty()
        {
            Assert.IsFalse(Chain.TryGetValue(TestEnum.Two, out var value));
            Assert.IsNull(value);
        }
    }
}
