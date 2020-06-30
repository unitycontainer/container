using System;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Information about the type registered in a container.
    /// </summary>
    public readonly struct ContainerRegistration : IContainerRegistration
    {
        public Type RegisteredType => throw new NotImplementedException();

        public string? Name => throw new NotImplementedException();

        public Type? MappedToType => throw new NotImplementedException();

        public object? Instance => throw new NotImplementedException();

        public ResolveDelegate<IResolveContext> Factory => throw new NotImplementedException();

        public LifetimeManager LifetimeManager => throw new NotImplementedException();
    }
}
