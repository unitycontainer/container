using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers;
using System.Linq;
using Unity;
using Unity.Extension;

namespace Container
{
    public partial class Interfaces
    {
        [TestMethod]
        public void IUC_Baseline()
        {
            // Validate
            Assert.IsNotNull(Container);
            Assert.AreEqual("root", Container.Name);
            Assert.AreEqual(2, Container.Registrations.ToArray().Length);
        }

        [TestMethod]
        public void Register_Array()
        {
            // Act
            Container.Register(Registrations);

            // Validate
            Assert.AreEqual(5993, Container.Registrations.ToArray().Length);
        }

        [TestMethod]
        public void Register_Array_Event()
        {
            // Arrange
            Container.AddExtension(ExtensionMethod);

            // Act
            Container.Register(Registrations);

            // Validate
            Assert.AreEqual(5993, Container.Registrations.ToArray().Length);
            Assert.AreEqual(Registrations.Length, count);
            Assert.AreEqual(1, called);
            Assert.AreSame(Container, sender);
        }

        [TestMethod]
        public void Register_Span()
        {
            // Arrange
            var rent = ArrayPool<RegistrationDescriptor>.Shared.Rent(Registrations.Length);
            Array.Copy(Registrations, rent, Registrations.Length);
            ReadOnlySpan<RegistrationDescriptor> span = rent;

            // Act
            Container.Register(span.Slice(0, Registrations.Length));
            ArrayPool<RegistrationDescriptor>.Shared.Return(rent);

            // Validate
            Assert.AreEqual(5993, Container.Registrations.ToArray().Length);
        }

        [TestMethod]
        public void Register_Span_Event()
        {
            // Arrange
            Container.AddExtension(ExtensionMethod);

            // Arrange
            var rent = ArrayPool<RegistrationDescriptor>.Shared.Rent(Registrations.Length);
            Array.Copy(Registrations, rent, Registrations.Length);
            ReadOnlySpan<RegistrationDescriptor> span = rent;

            // Act
            Container.Register(span.Slice(0, Registrations.Length));
            ArrayPool<RegistrationDescriptor>.Shared.Return(rent);

            // Validate
            Assert.AreEqual(5993, Container.Registrations.ToArray().Length);
            Assert.AreEqual(Registrations.Length, count);
            Assert.AreEqual(1, called);
            Assert.AreSame(Container, sender);
        }



        public void ExtensionMethod(ExtensionContext context) 
            => context.Registering += OnRegistration;

        private void OnRegistration(object container, in ReadOnlySpan<RegistrationDescriptor> registrations)
        {
            count = registrations.Length;
            called++;
            sender = container;
        }
    }
}
