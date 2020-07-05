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
        public Type RegisteredType { get; }

        public string? Name { get; }

        public LifetimeManager LifetimeManager { get; }

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


        #region Constructors

        public ContainerRegistration(Type type, LifetimeManager manager)
        {
            Name = null;
            RegisteredType = type;
            LifetimeManager = manager;
        }

        public ContainerRegistration(Type type, string? name, LifetimeManager manager)
        {
            Name            = name;
            RegisteredType  = type;
            LifetimeManager = manager;
        }

        #endregion
    }
}
