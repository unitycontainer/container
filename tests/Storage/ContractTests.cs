using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Storage.Tests
{
    [TestClass]
    public class ContractTests
    {
        string Name = "3.1415";

        [TestMethod]
        public void Baseline()
        {
            // Arrange
            var contract = new Contract();

            // Validate
            Assert.IsNull(contract.Type);
            Assert.IsNull(contract.Name);
            Assert.AreEqual(0, contract.HashCode);
        }

        [TestMethod]
        public void TypeContract()
        {
            // Arrange
            var contract = new Contract(typeof(object));

            // Validate
            Assert.AreEqual(typeof(object), contract.Type);
            Assert.IsNull(contract.Name);
            Assert.AreEqual(typeof(object).GetHashCode(), contract.HashCode);
        }

        [TestMethod]
        public void TypeNameContract()
        {
            var hash = typeof(object).GetHashCode() ^ Name.GetHashCode();

            // Arrange
            var contract = new Contract(typeof(object), Name);

            // Validate
            Assert.AreEqual(typeof(object), contract.Type);
            Assert.AreEqual(Name, contract.Name);
            Assert.AreEqual(hash, contract.HashCode);
        }


        [TestMethod]
        public void EqualContract()
        {
            var contract = new Contract(typeof(object), Name);

            // Validate
            Assert.IsFalse(contract.Equals(null));
            Assert.IsFalse(contract == null);
            Assert.IsTrue(contract != null);
            Assert.IsTrue(contract == new Contract(typeof(object), "3." + "1415"));
            Assert.IsFalse(contract == new Contract(typeof(string), Name));
            Assert.IsTrue(contract != new Contract(typeof(int), Name));
        }
    }
}
