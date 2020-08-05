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

            IUnityContainer _container;
            ResolverOverride[] _overrides;

            //Span<ResolveContext> Parent;
            
            public ResolveContext ResolveContext;

            #endregion


            #region Constructors

            public ContainerContext(IUnityContainer container, ResolverOverride[] overrides)
            {
               // Parent = default;

                _container = container;
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
