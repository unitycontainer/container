using System;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial struct PipelineContext
    {
        #region New Request

        public PipelineContext(UnityContainer container, ref Contract contract, object existing, RegistrationManager manager, ref RequestInfo request)
        {
            unsafe
            {
                _parent = IntPtr.Zero;
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = existing;
            Action = manager.Data!;

            Registration = manager;
            Container = container;
        }


        public PipelineContext(UnityContainer container, ref Contract contract, RegistrationManager manager, ref RequestInfo request)
        {
            unsafe
            {
                _parent = IntPtr.Zero;
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = default;
            Action = default;

            Registration = manager;
            Container = container;
        }

        #endregion


        #region Recursive Request

        public PipelineContext(UnityContainer container, ref Contract contract, RegistrationManager manager, ref PipelineContext parent)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _request = parent._request;
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = default;
            Action = default;

            Registration = manager;
            Container = container;
        }

        public PipelineContext(ref PipelineContext parent, object action, object? data = null)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _request = parent._request;
                _contract = parent._contract;
            }

            Target = data;
            Action = action;

            Registration = parent.Registration;
            Container = parent.Container;
        }


        private PipelineContext(ref PipelineContext parent, ref Contract contract, object? action)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _request = parent._request;
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Target = default;
            Action = action;

            Registration = parent.Registration;
            Container = parent.Container;
        }

        #endregion
    }
}
