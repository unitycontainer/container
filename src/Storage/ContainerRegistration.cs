using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Information about the type registered in a container.
    /// </summary>
    [DebuggerDisplay("Name = { Name }", Name = "{ RegisteredType.Name,nq }")]
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ContainerRegistration : IContainerRegistration
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly Contract _contract;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly RegistrationManager _manager;

        #endregion


        #region Constructors

        public ContainerRegistration(Type type, RegistrationManager manager)
        {
            _contract = new Contract(type);
            _manager  = manager;
        }

        public ContainerRegistration(Type type, string? name, RegistrationManager manager)
        {
            _contract = new Contract(type, name);
            _manager  = manager;
        }

        public ContainerRegistration(in Contract contract, RegistrationManager manager)
        {
            _contract = contract;
            _manager = manager;
        }

        #endregion


        #region IContainerRegistration

        public Type RegisteredType => _contract.Type;

        public string? Name => _contract.Name;

        public LifetimeManager LifetimeManager => (LifetimeManager)_manager;

        public Type? MappedToType =>
            RegistrationCategory.Type == _manager.Category
                ? (Type?)_manager.Data
                : null;

        public object? Instance =>
            RegistrationCategory.Instance == _manager.Category
                ? _manager.Data
                : null;

        public ResolveDelegate<IResolveContext>? Factory =>
            RegistrationCategory.Factory == _manager.Category 
                ? (ResolveDelegate<IResolveContext>?)_manager.Data
                : null;

        #endregion
    }
}
