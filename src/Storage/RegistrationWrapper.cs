using System;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Registration;

namespace Unity.Storage
{
    [DebuggerDisplay("RegisteredType={RegisteredType?.Name},    Name={Name},    MappedTo={RegisteredType == MappedToType ? string.Empty : MappedToType?.Name ?? string.Empty},    {LifetimeManager?.GetType()?.Name}")]
    internal sealed class RegistrationWrapper : IContainerRegistration
    {
        private readonly TransientLifetimeManager _transient = new TransientLifetimeManager();

        public RegistrationWrapper(Type type, ExplicitRegistration registration)
        {
            RegisteredType = type;
            Name = registration.Name;
            MappedToType = registration.Type ?? RegisteredType;
            LifetimeManager = registration.LifetimeManager ?? _transient;
        }

        public RegistrationWrapper(Type type, string? name, Type? mappedTo, LifetimeManager manager)
        {
            RegisteredType = type;
            Name = name;
            MappedToType    = mappedTo ?? RegisteredType;
            LifetimeManager = manager  ?? _transient;
        }

        public Type RegisteredType { get; }

        public string? Name { get; } 

        public Type? MappedToType { get; }

        public LifetimeManager LifetimeManager { get; }
    }
}
