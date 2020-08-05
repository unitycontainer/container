using System;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public partial struct ResolveContext : IResolveContext
    {
        private readonly ResolverOverride[] _overrides;

        internal UnityContainer _container;

        public ResolveContext(UnityContainer container, ResolverOverride[] overrides)
        {
            _container = container;
            _overrides = overrides;
            
            Contract = default;
        }

        public readonly Contract Contract { get; }

        public Type Type => Contract.Type;

        public string? Name => Contract.Name;


        public object? Resolve(Type type, string? name)
        {
            return null;
        }
    }
}
