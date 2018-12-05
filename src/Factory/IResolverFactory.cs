using System;
using Unity.Build;

namespace Unity.Factory
{
    public interface IResolverFactory
    {
        BuildDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IBuildContext;
    }


    public interface IResolverFactory<TFactory> : IResolverFactory
    {
    }
}
