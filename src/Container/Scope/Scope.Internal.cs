using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Unity.Container
{
    public abstract partial class Scope 
    {
        internal abstract int MoveNext(int index, Type type);

        internal abstract int IndexOf(Type type, int hash);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static T[] AllocateUninitializedArray<T>(int size)
        {
#if NET5_0
            return GC.AllocateUninitializedArray<T>(size);
#else
            return new T[size];
#endif
        }


        #region Entries

        [DebuggerDisplay("Type = { Registration._contract.Type?.FullName }, Name = { Registration._contract.Name }, Manager = {Internal.Manager}")]
        [StructLayout(LayoutKind.Explicit)]
        public struct Entry
        {
            #region Fields

            [FieldOffset(0)]
            public int Next;

            [FieldOffset(8)]
            public ContainerRegistration Registration;

            [FieldOffset(8)]
            internal InternalRegistration Internal;

            #endregion


            #region Constructors

            public Entry(int hash, Type type, string? name, RegistrationManager manager, int next = 0)
            {
                Next = next;
                Internal = default;
                Registration = new ContainerRegistration(hash, type, name, manager);
            }

            public Entry(int hash, Type type, RegistrationManager manager, int next = 0)
            {
                Next = next;
                Internal = default;
                Registration = new ContainerRegistration(hash, type, manager);
            }

            public Entry(int hash, Type type)
            {
                Next = 0;
                Registration = default;

                Internal.Manager = default;
                Internal.Contract = new Contract(hash, type);
            }

            public Entry(in Contract contract, RegistrationManager manager, int next)
            {
                Next = next;
                Internal = default;
                Registration = new ContainerRegistration(in contract, manager);
            }

            #endregion


            #region Properties

            public int HashCode => Internal.Contract.HashCode;

            public Type Type => Internal.Contract.Type;

            public string? Name => Internal.Contract.Name;

            public RegistrationManager? Manager => Internal.Manager;

            #endregion
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct InternalRegistration
        {
            public Contract Contract;
            public RegistrationManager? Manager;
        }

        #endregion
    }
}
