using System;

namespace Unity.Resolution
{
    public interface IResolverFactory
    {
        ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IResolveContext;
    }
}
