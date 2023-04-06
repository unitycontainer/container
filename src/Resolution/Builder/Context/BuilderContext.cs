using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Builder
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
   // [DebuggerDisplay("Context: Type: {Contract.Type?.Name},  Name: {Contract.Name},  Scope: {Container.Name}")]
    public partial struct BuilderContext : IBuilderContext
    {
        #region Fields

        private  readonly IntPtr _parent;
        private  readonly IntPtr _request;
        internal readonly IntPtr _error;
        internal readonly IntPtr _registration;

        private Type? _type;
        private object? _target;
        private IntPtr _contract;
        private bool _perResolve;
        private InjectionMember? _policies;
        internal RegistrationManager? _manager;

        #endregion


        #region Properties

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

            _manager = manager;
            _perResolve = manager is Lifetime.PerResolveLifetimeManager;

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

            Container = container;
            CurrentOperation = default;
        }

        #endregion


        #region Recursive Request

        private BuilderContext(ref Contract contract, ref ErrorDescriptor error, ref BuilderContext parent)
        {
            unsafe
            {
                _request = parent._request;
                _parent = new IntPtr(Unsafe.AsPointer(ref parent));
                _error = new IntPtr(Unsafe.AsPointer(ref error));
                _registration = _contract = new IntPtr(Unsafe.AsPointer(ref contract));
            }

            Container = parent.Container;
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

            _perResolve = perResolve;
            _registration = parent._contract;

            Container = parent.Container;
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

            Container = parent.Container;
        }

        #endregion
    }
}
