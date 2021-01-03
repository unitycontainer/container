using System;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial struct PipelineContext
    {
        #region New Request

        public PipelineContext(UnityContainer container, ref Contract contract, RegistrationManager manager, ref RequestInfo request)
        {
            unsafe
            {
                _parent = IntPtr.Zero;
                _error = new IntPtr(Unsafe.AsPointer(ref request.ErrorInfo));
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _registration = _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            _target = default;
            _perResolve = manager is Lifetime.PerResolveLifetimeManager;

            CurrentOperation = default;
            Container = container;
            Registration = manager;
        }

        public PipelineContext(UnityContainer container, ref Contract contract, ref RequestInfo request)
        {
            unsafe
            {
                _parent = IntPtr.Zero;
                _error = new IntPtr(Unsafe.AsPointer(ref request.ErrorInfo));
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _registration = _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            _target = default;
            _perResolve = false;

            Registration = default;
            Container = container;
            CurrentOperation = default;
        }

        #endregion


        #region Recursive Request

        private PipelineContext(UnityContainer container, ref Contract contract, RegistrationManager manager, ref PipelineContext parent)
        {
            unsafe
            {
                _error = parent._error;
                _request = parent._request;
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _registration = _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            _target = default;
            _perResolve = manager is Lifetime.PerResolveLifetimeManager;

            Registration = manager;
            Container = container;
            CurrentOperation = default;
        }

        private PipelineContext(UnityContainer container, ref Contract contract, ref PipelineContext parent)
        {
            unsafe
            {
                _error = parent._error;
                _request = parent._request;
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _registration = _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            _target = default;
            _perResolve = false;

            Registration = default;
            Container = container;
            CurrentOperation = default;
        }

        private PipelineContext(ref Contract contract, ref ErrorInfo error, ref PipelineContext parent)
        {
            unsafe
            {
                _request = parent._request;
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _error = new IntPtr(Unsafe.AsPointer(ref error));
                _registration = _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            _target = default;
            _perResolve = false;

            Registration = default;
            Container = parent.Container;
            CurrentOperation = default;
        }

        private PipelineContext(ref Contract contract, ref PipelineContext parent, bool perResolve)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _error = parent._error;
                _request = parent._request;
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            _target = default;
            _perResolve = perResolve;
            _registration = parent._contract;

            Registration = default;
            Container = parent.Container;
            CurrentOperation = default;
        }

        private PipelineContext(ref Contract contract, ref PipelineContext parent)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _error = parent._error;
                _request = parent._request;
                _registration = _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            _target = default;
            _perResolve = false;

            Registration = default;
            Container = parent.Container;
            CurrentOperation = default;
        }

        #endregion
    }
}
