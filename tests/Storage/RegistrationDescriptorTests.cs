using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;

namespace Storage.Tests
{
    [TestClass]
    public class RegistrationDescriptorTests
    {
        [TestMethod]
        public void Uninitialized()
        {
            // Arrange
            var registration = new RegistrationDescriptor();

            // Validate
            Assert.AreEqual(RegistrationCategory.Uninitialized, registration.Category);
            Assert.IsNull(registration.Type);
            Assert.IsNull(registration.Instance);
            Assert.IsNull(registration.Factory);
        }
    }
}
