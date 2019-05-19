using System;
using System.Diagnostics;
using Unity.Lifetime;

namespace Unity.Registration
{
    [DebuggerDisplay("RegisteredType={RegisteredType?.Name},    Name={Name},    MappedTo={RegisteredType == MappedToType ? string.Empty : MappedToType?.Name ?? string.Empty},    {LifetimeManager?.GetType()?.Name}")]
    internal sealed class ContainerRegistration : IContainerRegistration
    {
        private readonly ExplicitRegistration _registration;

        public ContainerRegistration(Type type, ExplicitRegistration registration)
        {
            RegisteredType = type;
            _registration = registration;
        }

        public Type RegisteredType { get; }

        public string? Name => _registration.Name;

        public Type? MappedToType => _registration.Type;

        public LifetimeManager LifetimeManager => _registration.LifetimeManager ??
                                                  TransientLifetimeManager.Instance;
    }
}
