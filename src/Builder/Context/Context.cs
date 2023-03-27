using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;

namespace Unity.Builder
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    [DebuggerDisplay("Context: Type: {Contract.Type?.Name},  Name: {Contract.Name},  Scope: {Container.Name}")]
    public partial struct BuilderContext : IBuilderContext
    {
        #region Fields

        private readonly IntPtr _error;
        private readonly IntPtr _parent;
        private readonly IntPtr _request;
        private readonly IntPtr _registration;

        private Type? _type;
        private Type? _generic;
        private object? _target;
        private IntPtr _contract;
        private bool _perResolve;
        private InjectionMember? _policies;
        private RegistrationManager? _manager;

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

            _type = default;
            _target = default;
            _manager = manager;
            _policies = default;
            _perResolve = manager is Lifetime.PerResolveLifetimeManager;

            CurrentOperation = default;
            Container = container;
            _generic = null;
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

            _type = default;
            _target = default;
            _manager = default;
            _policies = default;
            _perResolve = false;

            Container = container;
            CurrentOperation = default;
            _generic = null;
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

            _type = default;
            _target = default;
            _manager = default;
            _policies = default;
            _perResolve = false;

            Container = parent.Container;
            CurrentOperation = default;
            _generic = null;
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

            _type = default;
            _target = default;
            _manager = default;
            _perResolve = perResolve;
            _policies = default;
            _registration = parent._contract;

            Container = parent.Container;
            CurrentOperation = default;
            _generic = null;
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

            _type = default;
            _target = default;
            _manager = default;
            _policies = default;
            _perResolve = false;

            Container = parent.Container;
            CurrentOperation = default;
            _generic = null;
        }

        #endregion
    }
}
