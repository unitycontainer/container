using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Unity.Storage;

namespace Storage.Tests
{
    [TestClass]
    public class StagedChainTests
    {
        [TestMethod]
        public void Baseline()
        {
            Assert.AreEqual(0, (int)TestEnum.Zero);
            Assert.AreEqual(0, (int)TestIntEnum.Zero);
            Assert.AreEqual(0, (int)TestZeroedEnum.Zero);
        }

        [TestMethod]
        public void EmptyChain()
        {
            StagedChain<TestEnum, object> Chain = new StagedChain<TestEnum, object>(); 

            Assert.AreEqual(0, Chain.Count);
            Assert.AreEqual(0, Chain.Keys.Count);
            Assert.AreEqual(0, Chain.Values.Count);
            Assert.AreEqual(0, Chain.ToArray().Length);
        }

    }

    #region Test Data

    public enum TestEnum
    {
        Zero,
        One,
        Two,
        Three,
        Four
    }

    public enum TestIntEnum : int
    { 
        Zero, 
        One, 
        Two,
        Three,
        Four
    }

    public enum TestZeroedEnum
    {
        Zero = 0,
        One,
        Two,
        Three,
        Four
    }

    #endregion
}
