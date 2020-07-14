using System;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public readonly ref struct RegistrationData
    {
        #region Constructors

        internal RegistrationData(LifetimeManager manager, params Type[] registerAs)
        {
            RegisterAs = registerAs;
            Manager = manager;
            Name = null;
        }

        internal RegistrationData(string? name, LifetimeManager manager, params Type[] registerAs)
        {
            RegisterAs = registerAs;
            Manager = manager;
            Name = name;
        }

        public RegistrationData(Type type, string? name, ITypeLifetimeManager manager, params Type[] registerAs)
        {
            Name       = name;
            Manager    = (LifetimeManager)manager;
            RegisterAs = 0 < registerAs.Length ? registerAs : new[] { type };

            // Setup manager
            Manager.Data = type;
            Manager.RegistrationType = RegistrationType.Type;
        }

        public RegistrationData(object? instance, string? name, IInstanceLifetimeManager manager, params Type[] registerAs)
        {
            Name = name;
            Manager = (LifetimeManager)manager;
            RegisterAs = 0 < registerAs.Length 
                ? registerAs 
                : new[] { instance?.GetType() ?? throw new ArgumentException("registerAs must be provided when registering 'null'", nameof(registerAs)) };

            // Setup manager
            Manager.Data = instance;
            Manager.RegistrationType = RegistrationType.Instance;
        }

        public RegistrationData(ResolveDelegate<IResolveContext> factory, string? name, IFactoryLifetimeManager manager, params Type[] registerAs)
        {
            Name = name;
            Manager = (LifetimeManager)manager;
            RegisterAs = registerAs;

            // Setup manager
            Manager.Data = factory;
            Manager.RegistrationType = RegistrationType.Factory;

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
        public readonly LifetimeManager Manager;

        /// <summary>
        /// <see cref="RegistrationType"/> of this registration. 
        /// </summary>
        public readonly RegistrationType RegistrationType => Manager.RegistrationType;

        /// <summary>
        /// In <see cref="Type"/> registrations this holds registered <see cref="Type"/>
        /// </summary>
        /// <remarks>
        /// This parameter used to be called TypeTo. In other words, this is the <see cref="Type"/>
        /// that implements all the <see cref="RegisterAs"/> interfaces.
        /// </remarks>
        public readonly Type? Type 
            => RegistrationType.Type == Manager.RegistrationType 
                ? (Type?)Manager.Data : null;

        /// <summary>
        /// In instance registrations this is the instance that has been registered
        /// </summary>
        public readonly object? Instance 
            => RegistrationType.Instance == Manager.RegistrationType 
                ? Manager.Data : null;

        /// <summary>
        /// In factory registration this property holds the factory delegate
        /// </summary>
        public readonly ResolveDelegate<IResolveContext>? Factory 
            => RegistrationType.Factory == Manager.RegistrationType 
                ? (ResolveDelegate<IResolveContext>?)Manager.Data : null;

        #endregion

        public readonly override string ToString()
        {
            return $"Registration: {RegistrationType},  {Manager}";
        }
    }
}
