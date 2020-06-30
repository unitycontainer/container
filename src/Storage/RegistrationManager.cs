using System.Diagnostics;
using Unity.Injection;

namespace Unity
{
    /// <summary>
    /// This enumeration identifies type of registration 
    /// </summary>
    public enum RegistrationType
    {
        /// <summary>
        /// Initial, uninitialized state
        /// </summary>
        Uninitialized = 0,

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
    public abstract class RegistrationManager
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string? Name; // TODO

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal object? Data;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal RegistrationType   RegistrationType;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal InjectionMember[]? InjectionMembers;
    }
}
