using System;
using Unity.Build;
using Unity.Delegates;

namespace Unity.Policy
{
    public interface IResolverFactory
    {
        ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IBuildContext;
    }
}
