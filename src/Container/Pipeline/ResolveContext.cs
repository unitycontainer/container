using System;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public partial struct ResolveContext : IResolveContext
    {
        public UnityContainer? Container;
        public RegistrationManager? Manager;

        public readonly Contract Contract;
        private readonly ResolverOverride[] _overrides;

        public ResolveContext(UnityContainer container, Type type, string? name, ResolverOverride[] overrides)
        {
            Contract = new Contract(type, name);
            Container = container;
            Manager = null;

            _overrides = overrides;
        }

        public ResolveContext(Type type, string? name, ResolverOverride[] overrides)
        {
            Contract = new Contract(type, name);
            Container = default;
            Manager = null;

            _overrides = overrides;
        }




        public Type Type => Contract.Type;

        public string? Name => Contract.Name;


        public object? Resolve(Type type, string? name)
        {
            return null;
        }
    }
}
