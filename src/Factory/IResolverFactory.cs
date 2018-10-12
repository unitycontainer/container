using System;
using Unity.Build;
using Unity.Delegates;

namespace Unity.Factory
{
    public interface IResolverFactory
    {
        ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IBuildContext;
    }


    public interface IResolverFactory<TFactory> : IResolverFactory
    {
    }
}
