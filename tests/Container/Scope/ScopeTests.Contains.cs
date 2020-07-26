using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void ContainsEmptyTest()
        {
            // Arrange
            var type = Manager.GetType();

            // Validate
            Assert.IsFalse(Scope.Contains(type, type.Name));
        }

        [TestMethod]
        public void ContainsTest()
        {
            // Arrange
            ReadOnlySpan<RegistrationDescriptor> span = Registrations;

            // Act
            Scope.Add(span);

            // Validate
            foreach (var registration in Registrations)
            { 
                Assert.IsTrue(Scope.Contains(
                    registration.RegisterAs.First(), 
                    registration.Name));
            }
            Assert.IsFalse(Scope.Contains(typeof(ScopeTests), null));
        }
    }
}
