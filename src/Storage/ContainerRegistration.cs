using System;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Information about the type registered in a container.
    /// </summary>
    [DebuggerDisplay("Type = { RegisteredType.Name.PadRight(20),nq } Name = { Name ?? \"null\",nq }")]
    public struct ContainerRegistration : IContainerRegistration
    {
        public Type RegisteredType { get; internal set; }

        public string? Name { get; internal set; }

        public Type? MappedToType { get; internal set; }

        public object? Instance { get; internal set; }

        public ResolveDelegate<IResolveContext>? Factory { get; internal set; }

        public LifetimeManager LifetimeManager { get; internal set; }
    }
}
