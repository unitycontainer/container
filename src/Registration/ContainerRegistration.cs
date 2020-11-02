using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Lifetime;

namespace Unity
{
#if DEBUG
    [DebuggerDisplay("Hash = {_contract.HashCode}, Name = { Name }", Name = "{ (RegisteredType?.Name ?? string.Empty),nq }")]
#else
    [DebuggerDisplay("Name = { Name }", Name = "{ (RegisteredType?.Name ?? string.Empty),nq }")]
#endif
    /// <summary>
    /// Information about the type registered in a container.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ContainerRegistration : IContainerRegistration
    {
        #region Fields
        
        // Do not change the sequential order

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Contract _contract;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly RegistrationManager _manager;

        #endregion


        #region Constructors

        internal ContainerRegistration(int hash, Type type, string? name, RegistrationManager manager)
        {
            _contract = new Contract(hash, type, name);
            _manager = manager;
        }

        internal ContainerRegistration(in Contract contract, RegistrationManager manager)
        {
            _contract = contract;
            _manager = manager;
        }

        internal ContainerRegistration(int hash, Type type, RegistrationManager manager)
        {
            _contract = new Contract(hash, type);
            _manager = manager;
        }

        #endregion


        #region Properties

        public Type ContractType => _contract.Type;

        public string? ContractName => _contract.Name;

        public Type LifetimeManagerType => _manager.GetType();

        #endregion


        #region Legacy

        public Type RegisteredType => _contract.Type;

        public string? Name => _contract.Name;

        public LifetimeManager LifetimeManager => (LifetimeManager)_manager;

        public Type? MappedToType
        {
            get
            {
                if (null == _manager) return null;
                return _manager.Category switch
                {
                    RegistrationCategory.Type => (Type?)_manager.Data,
                    RegistrationCategory.Factory => RegisteredType,
                    RegistrationCategory.Instance => _manager.Data?.GetType() ?? RegisteredType,
                    _ => null
                };
            }
        }

        #endregion
    }
}
