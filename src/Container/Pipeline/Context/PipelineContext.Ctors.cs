using System;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial struct PipelineContext
    {
        #region New Request

        public PipelineContext(UnityContainer container, ref Contract contract, RegistrationManager manager, ref PipelineRequest request)
        {
            unsafe
            {
                _parent = IntPtr.Zero;
                _error = new IntPtr(Unsafe.AsPointer(ref request.Error));
                _request = new IntPtr(Unsafe.AsPointer(ref request));
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Type = manager.Type ?? contract.Type;
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
                _error  = parent._error;
                _request = parent._request;
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }


            Type = manager.Type ?? contract.Type;
            Target = default;
            Action = default;

            Registration = manager;
            Container = container;
        }

        public PipelineContext(ref PipelineContext parent, ref Contract contract, object? action)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _error  = parent._error;
                _request = parent._request;
                _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Type = contract.Type;
            Target = default;
            Action = action;

            Registration = parent.Registration;
            Container = parent.Container;
        }

        #endregion
    }
}
