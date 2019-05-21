using System;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Policy;

namespace Unity.Registration
{
    [DebuggerDisplay("RegisteredType={RegisteredType?.Name},    Name={Name},    MappedTo={RegisteredType == MappedToType ? string.Empty : MappedToType?.Name ?? string.Empty},    {LifetimeManager?.GetType()?.Name}")]
    internal sealed class ContainerRegistration : IContainerRegistration
    {
        private readonly ExplicitRegistration _registration;

        public ContainerRegistration(Type type, IPolicySet registration)
        {
            RegisteredType = type;
            _registration = (ExplicitRegistration)registration;
        }

        public Type RegisteredType { get; }

        public string? Name => _registration.Name;

        public Type? MappedToType => _registration.Type;

        public LifetimeManager LifetimeManager => _registration.LifetimeManager ??
                                                  TransientLifetimeManager.Instance;
    }
}
