using System;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Storage
{
    [DebuggerDisplay("RegisteredType={RegisteredType?.Name},    Name={Name},    MappedTo={RegisteredType == MappedToType ? string.Empty : MappedToType?.Name ?? string.Empty},    {LifetimeManager?.GetType()?.Name}")]
    internal sealed class RegistrationWrapper : IContainerRegistration
    {
        private readonly ExplicitRegistration _registration;

        public RegistrationWrapper(Type type, IPolicySet registration)
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
