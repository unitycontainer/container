using System;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public readonly ref struct RegistrationData
    {
        #region Fields

        public readonly Type[]  RegisterAs;
        public readonly string? Name;
        public readonly LifetimeManager Manager;

        #endregion

        #region Constructors

        internal RegistrationData(string? name, LifetimeManager manager, Type[] registerAs)
        {
            RegisterAs = registerAs;
            Manager = manager;
            Name = name;
        }

        public RegistrationData(Type type, string? name, ITypeLifetimeManager manager, Type[] registerAs)
        {
            Name       = name;
            Manager    = (LifetimeManager)manager;
            RegisterAs = 0 < registerAs.Length ? registerAs : new[] { type };

            // Setup manager
            Manager.Data = type;
            Manager.RegistrationType = RegistrationType.Type;
        }

        public RegistrationData(object? instance, string? name, IInstanceLifetimeManager manager, Type[] registerAs)
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

        public RegistrationData(ResolveDelegate<IResolveContext> factory, string? name, IFactoryLifetimeManager manager, Type[] registerAs)
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

        public RegistrationType RegistrationType => Manager.RegistrationType;

        public Type? Type 
            => RegistrationType.Type == Manager.RegistrationType 
                ? (Type?)Manager.Data : null;

        public object? Instance 
            => RegistrationType.Instance == Manager.RegistrationType 
                ? Manager.Data : null;

        public ResolveDelegate<IResolveContext>? Factory 
            => RegistrationType.Factory == Manager.RegistrationType 
                ? (ResolveDelegate<IResolveContext>?)Manager.Data : null;

        #endregion

        public override string ToString()
        {
            return $"Registration: {RegistrationType},  {Manager}";
        }
    }
}
