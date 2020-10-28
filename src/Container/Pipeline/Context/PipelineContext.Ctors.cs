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
                _error = new IntPtr(Unsafe.AsPointer(ref request.Error));
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = default;
            Action = default;

            Registration = manager;
            Container = container;
        }

        public PipelineContext(UnityContainer container, ref Contract contract, ref RequestInfo request)
        {
            unsafe
            {
                _parent = IntPtr.Zero;
                _error = new IntPtr(Unsafe.AsPointer(ref request.Error));
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = default;
            Action = default;

            Registration = default;
            Container = container;
        }

        #endregion


        #region Recursive Request

        private PipelineContext(UnityContainer container, ref Contract contract, RegistrationManager manager, ref PipelineContext parent)
        {
            unsafe
            {
                _error  = parent._error;
                _request = parent._request;
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = default;
            Action = default;

            Registration = manager;
            Container = container;
        }

        private PipelineContext(ref Contract contract, RegistrationManager manager, ref PipelineContext parent)
        {
            unsafe
            {
                _error = parent._error;
                _request = parent._request;
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = default;
            Action = default;

            Registration = manager;
            Container = parent.Container;
        }

        private PipelineContext(object action, ref Contract contract, ref ErrorInfo error, ref PipelineContext parent)
        {
            unsafe
            {
                _request  = parent._request;
                _parent   = new IntPtr(Unsafe.AsPointer(ref parent));
                _error    = new IntPtr(Unsafe.AsPointer(ref error)); 
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = default;
            Action = action;

            Registration = parent.Registration;
            Container = parent.Container;
        }

        private PipelineContext(ref Contract contract, ref ErrorInfo error, ref PipelineContext parent)
        {
            unsafe
            {
                _request = parent._request;
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _error = new IntPtr(Unsafe.AsPointer(ref error));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = default;
            Action = default;

            Registration = parent.Registration;
            Container = parent.Container;
        }

        private PipelineContext(object action, ref Contract contract, ref PipelineContext parent)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _error = parent._error;
                _request = parent._request;
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = default;
            Action = action;

            Registration = parent.Registration;
            Container = parent.Container;
        }

        private PipelineContext(ref Contract contract, ref PipelineContext parent)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _error = parent._error;
                _request = parent._request;
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = default;
            Action = default;

            Registration = parent.Registration;
            Container = parent.Container;
        }

        #endregion
    }
}
