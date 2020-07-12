using System;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Information about the type registered in a container.
    /// </summary>
    [DebuggerDisplay("Name = { Name }", Name = "{ RegisteredType.Name,nq }")]
    public readonly struct ContainerRegistration : IContainerRegistration
    {
        #region Fields

        internal readonly Contract        _contract;
        internal readonly LifetimeManager _manager;

        #endregion


        #region Constructors

        public ContainerRegistration(in Contract contract, LifetimeManager manager)
        {
            _contract = contract;
            _manager  = manager;
        }

        public ContainerRegistration(Type type, LifetimeManager manager)
        {
            _contract = new Contract(type);
            _manager  = manager;
        }

        public ContainerRegistration(Type type, string? name, LifetimeManager manager)
        {
            _contract = new Contract(type, name);
            _manager  = manager;
        }

        #endregion


        #region IContainerRegistration

        public Type RegisteredType => _contract.Type;

        public string? Name => _contract.Name;

        public LifetimeManager LifetimeManager => _manager;

        public Type? MappedToType =>
            RegistrationType.Type == _manager.RegistrationType
                ? (Type?)_manager.Data
                : null;

        public object? Instance =>
            RegistrationType.Instance == _manager.RegistrationType
                ? _manager.Data
                : null;

        public ResolveDelegate<IResolveContext>? Factory =>
            RegistrationType.Factory == _manager.RegistrationType 
                ? (ResolveDelegate<IResolveContext>?)_manager.Data
                : null;

        #endregion
    }
}
