using System;

namespace Unity.Resolution
{
    public interface IResolverFactory<in TMemberInfo>
    {
        ResolveDelegate<TContext> GetResolver<TContext>(TMemberInfo info)
            where TContext : IResolveContext;
    }
}
