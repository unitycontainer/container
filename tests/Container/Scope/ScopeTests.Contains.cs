using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity;

namespace Container.Scopes
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void ContainsEmptyTest()
        {
            // Arrange
            var type = Manager.GetType();

            // Validate
            Assert.IsFalse(Scope.Contains(new Contract( type, type.Name)));
        }

        [TestMethod]
        public void ContainsTest()
        {
            // Arrange
            ReadOnlySpan<RegistrationDescriptor> span = Registrations;

            // Act
            Scope.Register(span);

            // Validate
            foreach (var registration in Registrations)
            { 
                Assert.IsTrue(Scope.Contains(new Contract(
                    registration.RegisterAs.First(), 
                    registration.Name)));
            }
            Assert.IsFalse(Scope.Contains(new Contract(typeof(ScopeTests), null)));
        }
    }
}
