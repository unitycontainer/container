using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Unity.Container
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [DebuggerDisplay("Context: Type: {Type.Name},  Name: {Name},  Scope: {Container.Name}")]
    public partial struct BuilderContext
    {
        #region Fields

        private readonly IntPtr _error;
        private readonly IntPtr _parent;
        private readonly IntPtr _request;
        private readonly IntPtr _registration;

        private bool _perResolve;
        private IntPtr _contract;
        private object? _target;
        private RegistrationManager? _manager;

        public UnityContainer Container { get; private set; }
        public object? CurrentOperation { get; set; }


        #endregion


        #region New Request

        private BuilderContext(UnityContainer container, ref Contract contract, RegistrationManager manager, ref RequestInfo request)
        {
            unsafe
            {
                _parent = IntPtr.Zero;
                _error = new IntPtr(Unsafe.AsPointer(ref request.ErrorInfo));
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _registration = _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            _target = default;
            _manager = manager;
            _perResolve = manager is Lifetime.PerResolveLifetimeManager;

            CurrentOperation = default;
            Container = container;
        }

        private BuilderContext(UnityContainer container, ref Contract contract, ref RequestInfo request)
        {
            unsafe
            {
                _parent = IntPtr.Zero;
                _error = new IntPtr(Unsafe.AsPointer(ref request.ErrorInfo));
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _registration = _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            _target = default;
            _manager = default;
            _perResolve = false;

            Container = container;
            CurrentOperation = default;
        }

        #endregion


        #region Recursive Request

        private BuilderContext(ref Contract contract, ref ErrorInfo error, ref BuilderContext parent)
        {
            unsafe
            {
                _request = parent._request;
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _error = new IntPtr(Unsafe.AsPointer(ref error));
                _registration = _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            _target = default;
            _manager = default;
            _perResolve = false;

            Container = parent.Container;
            CurrentOperation = default;
        }

        private BuilderContext(ref Contract contract, ref BuilderContext parent, bool perResolve)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _error = parent._error;
                _request = parent._request;
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            _target = default;
            _manager = default;
            _perResolve = perResolve;
            _registration = parent._contract;

            Container = parent.Container;
            CurrentOperation = default;
        }

        private BuilderContext(ref Contract contract, ref BuilderContext parent)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _error = parent._error;
                _request = parent._request;
                _registration = _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            _target = default;
            _manager = default;
            _perResolve = false;

            Container = parent.Container;
            CurrentOperation = default;
        }

        #endregion
    }
}
