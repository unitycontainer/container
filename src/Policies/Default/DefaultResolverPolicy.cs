using Unity.Container;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Policies.Default
{
    public class DefaultResolverPolicy : IResolverPolicy
    {
        public object Resolve(IContainerContext context, ResolverOverride[] resolverOverrides = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
