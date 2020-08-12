using System;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public partial struct ResolveContext : IResolveContext
    {
        public UnityContainer? Container;
        public RegistrationManager? Manager;

        public readonly Contract Contract;
        private readonly ResolverOverride[] Overrides;

        #region Constructors


        public ResolveContext(UnityContainer container, Type type, string? name, ResolverOverride[] overrides)
        {
            Contract = new Contract(type, name);
            Container = container;
            Manager = default;
            Existing = default;
            Overrides = overrides;
        }

        public ResolveContext(UnityContainer container, in Contract contract, ResolverOverride[] overrides)
        {
            Contract  = contract;
            Container = container;
            Manager = null;
            Existing = null;

            Overrides = overrides;
        }

        #endregion


        public object? Existing;


        #region IResolveContext

        public Type Type => Contract.Type;

        public string? Name => Contract.Name;


        public object? Resolve(Type type, string? name)
        {
            return null;
        }
        
        #endregion
    }
}
