using System;
using Unity.Pipeline;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        public ref struct ContainerContext
        {
            #region Fields

            public readonly Contract Contract;
            public IUnityContainer? Container;
            public RegistrationManager? Manager;

            ResolverOverride[] _overrides;

            //Span<ResolveContext> Parent;
            
            public ResolveContext ResolveContext;

            #endregion


            #region Constructors


            public ContainerContext(UnityContainer container, Type type, string? name, ResolverOverride[] overrides)
            {
                // Parent = default;
                Contract = new Contract(type, name);
                Container = container;
                Manager = null;

                _overrides = overrides;

                ResolveContext = default;
            }

            public ContainerContext(UnityContainer container, in Contract contract, ResolverOverride[] overrides)
            {
                // Parent = default;
                Contract = contract;
                Container = container;
                Manager = null;

                _overrides = overrides;

                ResolveContext = default;
            }

            #endregion



            public object? Resolve(Type type, string? name)
            {
                
                
                
                throw new NotImplementedException();
            }
        }
    }
}
