using System;
using Unity.Injection;

namespace Unity
{
    /// <summary>
    /// This enumeration identifies type of registration 
    /// </summary>
    public enum RegistrationType
    { 
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
        public readonly object            Scope;
        public readonly Type[]            Interfaces;
        public readonly string?           Name;
        public readonly object            Data;
        public readonly RegistrationType  RegistrationType;
        public readonly InjectionMember[] InjectionMembers;

        internal RegistrationData(object scope, Type[] interfaces, string? name, object data, RegistrationType registrationType, InjectionMember[] injectionMembers)
        {
            Scope            = scope;
            Interfaces       = interfaces;
            Name             = name;
            Data             = data;
            RegistrationType = registrationType;
            InjectionMembers = injectionMembers;
        }
    }
}
