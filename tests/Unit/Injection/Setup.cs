using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Storage;

namespace Injection
{
    [TestClass]
    public partial class Sequence
    {
        #region Constants

        protected const string INJECTION = "Injection";

        #endregion


        [TestMethod]
        public void Baseline()
        {
            var ctor = new TestClass 
            { 
                Next = new TestClass() 
            };

            var segment = ctor as ISequence<TestClass>;

            Assert.IsNotNull(segment);
            Assert.AreEqual(2, segment.Count());

            Assert.AreSame(ctor, ctor[0]);
            Assert.AreSame(ctor.Next, ctor[1]);
        }
    }


    public class TestClass : ISequence<TestClass>
    {
        public TestClass this[int index] 
            => (0 == index)
            ? this : (Next ?? throw new ArgumentOutOfRangeException())[index - 1];

        public TestClass Next { get; set; }

        public int Length => (Next?.Length ?? 0) + 1;

        ISequenceSegment ISequenceSegment.Next { get => Next; set => Next = (TestClass)value; }
    }
}
