using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Container.Registrations
{
    [TestClass]
    public class EnumerationTests
    {
        [TestMethod]
        public void Baseline()
        {
            // Arrange
            var list = new List<object> { new object(), new object() };
            var enumerator = list.AsEnumerable();

            // Validate
            Assert.AreEqual(2, enumerator.Count());

            list.Add(new object());
            Assert.AreEqual(3, enumerator.Count());
        }
    }
}
