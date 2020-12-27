using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Unity;

#pragma warning disable CS1998 // Asynchronous method lacks 'await' operators and will run synchronously

namespace Container
{
    public partial class Interfaces
    {
        [TestMethod]
        public virtual async Task RegisterAsync_Array()
        {
            Assert.ThrowsException<AggregateException>(() => Container.RegisterAsync(Registrations).AsTask().Wait());
        }

        [TestMethod]
        public virtual async Task RegisterAsync_Memory()
        {
            ReadOnlyMemory<RegistrationDescriptor> memory = new ReadOnlyMemory<RegistrationDescriptor>(Registrations);

            Assert.ThrowsException<AggregateException>(() => Container.RegisterAsync(memory).AsTask().Wait());
        }
    }
}

#pragma warning restore CS1998