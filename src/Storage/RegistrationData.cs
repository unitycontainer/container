using System;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// This enumeration identifies type of registration 
    /// </summary>
    public enum RegistrationType
    {
        /// <summary>
        /// This is implicit/internal registration
        /// </summary>
        Internal,

        /// <summary>
        /// This is RegisterType registration
        /// </summary>
        Type,

        /// <summary>
        /// This is RegisterInstance registration
        /// </summary>
        Instance,

        /// <summary>
        /// This is RegisterFactory registration
        /// </summary>
        Factory
    }

    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public struct RegistrationData
    {
        public readonly object Scope;
        public readonly Type[]? Interfaces;
        public readonly string? Name;
        public readonly object Data;
        public readonly RegistrationType RegistrationType;
        public readonly InjectionMember[] InjectionMembers;
        public ResolveDelegate<IResolveContext>? Resolver;

        public RegistrationData(RegistrationData parent)
        {
            Scope            = parent.Scope;
            Interfaces       = null;
            Name             = null;
            Data             = parent.Data;
            RegistrationType = RegistrationType.Internal;
            InjectionMembers = parent.InjectionMembers;
            Resolver         = null;
        }

        internal RegistrationData(object scope, Type[] interfaces, string? name, Type type, InjectionMember[] injectionMembers)
        {
            Scope = scope;
            Interfaces = (null == interfaces || 0 == interfaces.Length) ? null : interfaces;
            Name = name;
            Data = type;
            RegistrationType = RegistrationType.Type;
            InjectionMembers = injectionMembers;
            Resolver = null;
        }

        internal RegistrationData(object scope, Type[] interfaces, string? name, object instance, InjectionMember[] injectionMembers)
        {
            Scope = scope;
            Interfaces = (null == interfaces || 0 == interfaces.Length) ? null : interfaces;
            Name = name;
            Data = instance;
            RegistrationType = RegistrationType.Instance;
            InjectionMembers = injectionMembers;
            Resolver = null;
        }

        internal RegistrationData(object scope, Type[] interfaces, string? name, ResolveDelegate<IResolveContext> factory, InjectionMember[] injectionMembers)
        {
            Scope = scope;
            Interfaces = (null == interfaces || 0 == interfaces.Length) ? null : interfaces;
            Name = name;
            Data = factory;
            RegistrationType = RegistrationType.Factory;
            InjectionMembers = injectionMembers;
            Resolver = null;
        }
    }
}
