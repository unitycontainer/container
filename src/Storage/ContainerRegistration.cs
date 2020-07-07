using System;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Information about the type registered in a container.
    /// </summary>
    [DebuggerDisplay("Name = {  Name }", Name = "{ (RegisteredType?.Name ?? string.Empty),nq }")]
    public struct ContainerRegistration : IContainerRegistration
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal int _hash;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal Type _type;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string? _name;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal RegistrationManager _manager;

        #endregion


        #region IContainerRegistration

        public Type RegisteredType => _type;

        public string? Name => _name;

        public LifetimeManager LifetimeManager => (LifetimeManager)_manager;

        public Type? MappedToType =>
            RegistrationType.Type == LifetimeManager?.RegistrationType
                ? (Type?)LifetimeManager.Data
                : null;

        public object? Instance =>
            RegistrationType.Instance == LifetimeManager?.RegistrationType
                ? LifetimeManager.Data
                : null;

        public ResolveDelegate<IResolveContext>? Factory =>
            RegistrationType.Factory == LifetimeManager?.RegistrationType 
                ? (ResolveDelegate<IResolveContext>?)LifetimeManager.Data
                : null;

        #endregion


        #region Implementation

        public override int GetHashCode()
        {
            return null == _name
                ? _type.GetHashCode()
                : _type.GetHashCode() ^ (_name.GetHashCode() * 17);

        }

        public static int GetHashCode(Type type, string? name)
        {
            return null == name
                ? type.GetHashCode()
                : type.GetHashCode() ^ (name.GetHashCode() * 17);

        }

        #endregion
    }
}
