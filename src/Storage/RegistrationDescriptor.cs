using System;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity 
{
    /// <summary>
    /// Structure holding registration data
    /// </summary>
    [DebuggerDisplay("Category = { Category }, { Manager?.Data }", Name = "{ Name,nq }")]
    public readonly struct RegistrationDescriptor
    {
        #region Constructors

        internal RegistrationDescriptor(LifetimeManager manager, params Type[] registerAs)
        {
            RegisterAs = registerAs;
            Manager = manager;
            Name = null;
        }

        internal RegistrationDescriptor(string? name, LifetimeManager manager, params Type[] registerAs)
        {
            RegisterAs = registerAs;
            Manager = manager;
            Name = name;
        }

        /// <summary>
        /// Register <see cref="Type"/>
        /// </summary>
        /// <param name="type"><see cref="Type"/> to register</param>
        /// <param name="name">Name of the contract</param>
        /// <param name="manager"><see cref="LifetimeManager"/> that implements <see cref="ITypeLifetimeManager"/></param>
        /// <param name="registerAs">Collection of interfaces/aliases this type should be registered under</param>
        public RegistrationDescriptor(Type type, string? name, ITypeLifetimeManager manager, params Type[] registerAs)
        {
            Name       = name;
            Manager    = (LifetimeManager)manager;
            RegisterAs = 0 < registerAs.Length ? registerAs : new[] { type };

            // Setup manager
            Manager.Data = type;
            Manager.Category = RegistrationCategory.Type;
        }

        /// <summary>
        /// Register instance
        /// </summary>
        /// <param name="instance">The instance to register</param>
        /// <param name="name">Name of the contract</param>
        /// <param name="manager"><see cref="LifetimeManager"/> that implements <see cref="IInstanceLifetimeManager"/></param>
        /// <param name="registerAs">Collection of interfaces/aliases this instance should be registered under</param>
        public RegistrationDescriptor(object? instance, string? name, IInstanceLifetimeManager manager, params Type[] registerAs)
        {
            Name = name;
            Manager = (LifetimeManager)manager;
            RegisterAs = 0 < registerAs.Length 
                ? registerAs 
                : new[] { instance?.GetType() ?? throw new ArgumentException("registerAs must be provided when registering 'null'", nameof(registerAs)) };

            // Setup manager
            Manager.Data = instance;
            Manager.Category = RegistrationCategory.Instance;
        }

        /// <summary>
        /// Register factory
        /// </summary>
        /// <param name="factory">Factory <see cref="Delegate"/></param>
        /// <param name="name">Name of the contract</param>
        /// <param name="manager"><see cref="LifetimeManager"/> that implements <see cref="IFactoryLifetimeManager"/></param>
        /// <param name="registerAs">Collection of types this factory creates</param>
        public RegistrationDescriptor(ResolveDelegate<IResolveContext> factory, string? name, IFactoryLifetimeManager manager, params Type[] registerAs)
        {
            Name = name;
            Manager = (LifetimeManager)manager;
            RegisterAs = registerAs;

            // Setup manager
            Manager.Data = factory;
            Manager.Category = RegistrationCategory.Factory;

            if (0 == registerAs.Length)
                throw new ArgumentException("Factory registration requires registerAs type", nameof(registerAs));
        }

        #endregion


        #region Public Members

        /// <summary>
        /// Set of <see cref="Type"/> aliases this registration is registered under
        /// </summary>
        /// <remarks> 
        /// This used to be known as TypeFrom parameter. 
        /// The list of implemented interfaces or base classes the registered entity
        /// implements and could be assigned to.
        /// </remarks>
        public readonly Type[] RegisterAs;
        
        /// <summary>
        /// Name of contract
        /// </summary>
        public readonly string? Name;
        
        /// <summary>
        /// Lifetime Manager
        /// </summary>
        public readonly RegistrationManager Manager;

        /// <summary>
        /// <see cref="RegistrationCategory"/> of registration. 
        /// </summary>
        public readonly RegistrationCategory Category => Manager?.Category ?? RegistrationCategory.Uninitialized;

        /// <summary>
        /// In <see cref="Type"/> registrations this holds registered <see cref="Type"/>
        /// </summary>
        /// <remarks>
        /// This parameter used to be called TypeTo. In other words, this is the <see cref="Type"/>
        /// that implements all the <see cref="RegisterAs"/> interfaces.
        /// </remarks>
        public readonly Type? Type 
            => RegistrationCategory.Type == (Manager?.Category ?? RegistrationCategory.Uninitialized)
                ? (Type?)Manager!.Data : null;

        /// <summary>
        /// In instance registrations this is the instance that has been registered
        /// </summary>
        public readonly object? Instance 
            => RegistrationCategory.Instance == (Manager?.Category ?? RegistrationCategory.Uninitialized)
                ? Manager!.Data : null;

        /// <summary>
        /// In factory registration this property holds the factory delegate
        /// </summary>
        public readonly ResolveDelegate<IResolveContext>? Factory 
            => RegistrationCategory.Factory == (Manager?.Category ?? RegistrationCategory.Uninitialized)
                ? (ResolveDelegate<IResolveContext>?)Manager!.Data : null;

        #endregion


        #region Implementation

        public readonly override string ToString()
        {
            return $"Registration: {Category},  {Manager}";
        }

        #endregion
    }
}
