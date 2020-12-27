using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity;

namespace Container
{
    public partial class Scopes
    {
        [TestMethod, TestProperty(TESTING, TRAIT_CONTAINS)]
        public void ContainsEmptyTest()
        {
            // Arrange
            var type = Manager.GetType();

            // Validate
            Assert.IsFalse(Scope.Contains(new Contract( type, type.Name)));
        }

        [TestMethod, TestProperty(TESTING, TRAIT_CONTAINS)]
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
            Assert.IsFalse(Scope.Contains(new Contract(typeof(Scopes), null)));
        }
    }
}
