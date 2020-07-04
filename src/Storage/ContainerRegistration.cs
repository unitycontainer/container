using System;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Information about the type registered in a container.
    /// </summary>
    [DebuggerDisplay("{ Name }", Name = "{ RegisteredType.Name,nq }")]
    public readonly struct ContainerRegistration : IContainerRegistration
    {
        public Type RegisteredType { get; }

        public string? Name { get; }

        public Type? MappedToType { get; }

        public object? Instance { get; }

        public ResolveDelegate<IResolveContext>? Factory { get; }

        public LifetimeManager LifetimeManager { get; }


        #region Constructors

        public ContainerRegistration(Type type, string? name, LifetimeManager manager)
        {
            Name            = name;
            RegisteredType  = type;
            LifetimeManager = manager;

            switch (manager.RegistrationType)
            {
                case RegistrationType.Type:
                    Factory      = null;
                    Instance     = null;
                    MappedToType = (Type?)manager.Data;
                    break;

                case RegistrationType.Instance:
                    Factory      = null;
                    Instance     = manager.Data;
                    MappedToType = null;
                    break;

                case RegistrationType.Factory:
                    Factory      = (ResolveDelegate<IResolveContext>?)manager.Data;
                    Instance     = null;
                    MappedToType = null;
                    break;

                default:
                    throw new InvalidOperationException("Manager is not registered");
            }
        }

        #endregion
    }
}
