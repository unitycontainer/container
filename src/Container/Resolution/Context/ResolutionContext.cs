using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.Resolution
{
    public partial struct ResolutionContext
    {
        #region Fields

        private readonly IntPtr _parent;
        private readonly IntPtr _request;
        private readonly IntPtr _contract;

        public readonly UnityContainer Container;
        public readonly RegistrationManager? Manager;

        public object? Data;

        public object? Existing;

        #endregion


        #region Constructors

        public ResolutionContext(UnityContainer container, ref Contract contract, RegistrationManager manager, ref RequestInfo request)
        {
            unsafe
            {
                _parent = IntPtr.Zero;
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Data = default;
            Manager = manager;
            Container = container;
            Existing = default;
        }

        public ResolutionContext(ref RequestInfo request, ref Contract contract, UnityContainer container)
        {
            unsafe
            {
                _parent = IntPtr.Zero;
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Data = default;
            Manager = default;
            Container = container;
            Existing = default;
        }

        private ResolutionContext(ref ResolutionContext parent)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _request = parent._request;
                _contract = parent._contract;
            }

            Data = default;
            Manager = default;
            Container = parent.Container;
            Existing = default;
        }


        #endregion


        #region Parent / Child Context

        internal readonly bool IsRoot => IntPtr.Zero == _parent;

        internal readonly ref ResolutionContext Next
        {
            get
            {
                unsafe
                {
                    Debug.Assert(IntPtr.Zero != _parent);
                    return ref Unsafe.AsRef<ResolutionContext>(_parent.ToPointer());
                }
            }
        }

        internal ResolutionContext CreateChildContext()
        {
            var self = this;
            return new ResolutionContext(ref self);
        }

        #endregion
    }
}
