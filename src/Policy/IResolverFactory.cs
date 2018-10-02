using Unity.Build;
using Unity.Delegates;

namespace Unity.Policy
{
    public interface IResolverFactory
    {
        ResolveDelegate<TContext> GetResolver<TContext>(ref TContext context) 
            where TContext : IBuildContext;
    }
}
