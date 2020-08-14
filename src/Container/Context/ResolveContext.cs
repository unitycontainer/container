using System;
using System.Runtime.InteropServices;
using Unity.Resolution;

namespace Unity.Container
{
    [StructLayout(LayoutKind.Sequential)]
    public partial struct ResolveContext : IResolveContext
    {
        public readonly Contract Contract;
        public readonly UnityContainer? Container;
        public readonly RegistrationManager? Manager;
        public readonly ResolverOverride[] Overrides;
        public object? Existing;


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

        public ResolveContext(UnityContainer container, in Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            Contract = contract;
            Container = container;
            Manager = manager;
            Existing = null;

            Overrides = overrides;
        }

        #endregion


        #region IResolveContext

        public readonly Type Type => Contract.Type;

        public readonly string? Name => Contract.Name;

        public readonly object? Resolve(Type type, string? name)
        {
            return null;
        }
        
        #endregion
    }
}
