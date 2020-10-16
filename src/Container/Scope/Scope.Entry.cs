using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        [DebuggerDisplay("Type = { Registration._contract.Type?.Name }, Name = { Registration._contract.Name }")]
        [StructLayout(LayoutKind.Explicit)]
        public struct Entry
        {
            #region Fields

            [FieldOffset(0)]
            public long Next;

            [FieldOffset(8)]
            public ContainerRegistration Registration;

            [FieldOffset(8)]
            internal InternalRegistration Internal;

            #endregion


            #region Constructors

            public Entry(int hash, Type type, string? name, RegistrationManager manager)
            {
                Next = 0;
                Internal = default;
                Registration = new ContainerRegistration(hash, type, name, manager);
            }

            public Entry(int hash, Type type, RegistrationManager manager, long next = 0)
            {
                Next = next;
                Internal = default;
                Registration = new ContainerRegistration(hash, type, manager);
            }

            public Entry(int hash, Type type)
            {
                Next = 0;
                Internal = default;
                Registration = default;

                Internal.Contract = new Contract(hash, type);
            }

            public Entry(in Contract contract, RegistrationManager manager, long next)
            {
                Next = next;
                Internal = default;
                Registration = new ContainerRegistration(in contract, manager);
            }

            #endregion
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct InternalRegistration
        {
            public Contract             Contract;
            public RegistrationManager? Manager;
        }
    }
}
