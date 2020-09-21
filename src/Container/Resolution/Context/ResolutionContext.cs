using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.Resolution
{
    public partial struct ResolutionContext : IResolveContext
    {
        #region Fields

        private object? _storage;
        private readonly IntPtr _target;

        private readonly IntPtr _parent;
        private readonly IntPtr _request;
        private readonly IntPtr _contract;

        public readonly UnityContainer Container;
        public readonly RegistrationManager? Manager;

        public bool IsFaulted; 
        public object? Data;

        #endregion


        #region Constructors

        public ResolutionContext(UnityContainer container, ref Contract contract, RegistrationManager manager, ref RequestInfo request)
        {
            _storage = null;

            unsafe
            {
                _parent = IntPtr.Zero;
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _target = new IntPtr(Unsafe.AsPointer(ref _storage));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Data = default;
            Manager = manager;
            Container = container;
            Existing = default;
            IsFaulted = false;
        }

        public ResolutionContext(ref RequestInfo request, ref Contract contract, UnityContainer container)
        {
            _storage = null;

            unsafe
            {
                _parent = IntPtr.Zero;
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _target = new IntPtr(Unsafe.AsPointer(ref _storage));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Data = default;
            Manager = default;
            Container = container;
            Existing = default;
            IsFaulted = false;
        }

        private ResolutionContext(ref ResolutionContext parent)
        {
            _storage = null;

            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _request = parent._request;
                _target = new IntPtr(Unsafe.AsPointer(ref _storage));
                _contract = parent._contract;
            }

            Data = default;
            Manager = default;
            Container = parent.Container;
            Existing = default;
            IsFaulted = false;
        }


        //public ResolutionContext(ref object storage)
        //{
        //    _storage = null;

        //    unsafe
        //    {
        //        _element = new IntPtr(Unsafe.AsPointer(ref storage));
        //    }
        //}



        #endregion


        #region Properties

        public ref object? Target
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<object?>(_target.ToPointer());
                }
            }
        }

        public object? Existing { get; set; }

        public ICollection<IDisposable> Scope => Container._scope.Disposables;

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
