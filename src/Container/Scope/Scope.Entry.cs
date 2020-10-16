using System;
using System.Diagnostics;
using Unity.Lifetime;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        [DebuggerDisplay("Type = { Registration._contract.Type?.Name }, Name = { Registration._contract.Name }  ")]
        public struct Entry
        {
            public int Next;
            public ContainerRegistration Registration;

            public Entry(int hash, Type type, string? name, RegistrationManager manager)
            {
                Next = 0;
                Registration = new ContainerRegistration(hash, type, name, manager);
            }

            public Entry(in Contract contract, RegistrationManager manager)
            {
                Next = 0;
                Registration = new ContainerRegistration(in contract, manager);
            }

            public Entry(Type type)
            {
                Next = 0;
                Registration = new ContainerRegistration(type, new TransientLifetimeManager());
            }

        }
    }
}
