using System;
using Unity.Resolution;

namespace Unity.Policy
{
    public interface IResolverFactory
    {
        ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IResolveContext;
    }
}
