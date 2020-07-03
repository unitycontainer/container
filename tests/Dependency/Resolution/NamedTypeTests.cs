using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Resolution;

namespace Resolution.Basics
{
    [TestClass]
    public class NamedTypeTests
    {
        private static Type  TestType = typeof(NamedTypeTests);
        private const string TestName = "98a884c0-7eae-4926-8f3e-bf1f42a05be6";

        [TestMethod]
        public void HashCodeNotZeroTest()
        {
            // Arrange
            NamedType structure = new NamedType();

            // Validate
            Assert.AreNotEqual(0, structure.GetHashCode());
        }

        [TestMethod]
        public void HashCodeRepeatableTest()
        {
            // Arrange
            var typeHash = TestType.GetHashCode();
            var nameHash = TestName.GetHashCode();

            NamedType structure1 = new NamedType(TestType, TestName);
            NamedType structure2 = new NamedType(typeof(NamedTypeTests), "98a884c0-7eae-4926-8f3e-bf1f42a05be6");

            // Validate
            Assert.AreEqual(structure1.GetHashCode(), structure2.GetHashCode());
            Assert.AreEqual(structure1.GetHashCode(), NamedType.GetHashCode(typeHash, nameHash));
            Assert.AreEqual(NamedType.GetHashCode(typeHash, nameHash), NamedType.GetHashCode(typeHash, nameHash));
        }


        [TestMethod]
        public void EqualsTest()
        {
            // Arrange
            NamedType structure1 = new NamedType(TestType, TestName );
            NamedType structure2 = new NamedType(typeof(NamedTypeTests), "98a884c0-7eae-4926-8f3e-bf1f42a05be6");
            NamedType structure3 = new NamedType(typeof(NamedTypeTests));

            // Validate
            Assert.IsTrue( structure1.Equals(structure2));
            Assert.IsFalse(structure1.Equals(structure3));

            Assert.IsTrue(structure1 == structure2);
            Assert.IsTrue(structure2 != structure3);

            Assert.IsFalse(structure1 != structure2);
            Assert.IsFalse(structure2 == structure3);
        }


        [TestMethod]
        public void ToStringTest()
        {
            // Arrange
            NamedType structure = new NamedType(TestType, TestName);
            var data = structure.ToString();

            // Validate
            Assert.IsTrue(data.Contains(TestName));
            Assert.IsTrue(data.Contains(TestType.Name));
        }

        [TestMethod]
        public void ToStringCold()
        {
            // Arrange
            NamedType structure = new NamedType();
            var data = structure.ToString();

            // Validate
            Assert.IsFalse(data.Contains(TestName));
            Assert.IsFalse(data.Contains(TestType.Name));
        }
    }
}
