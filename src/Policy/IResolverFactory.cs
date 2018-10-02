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

    public interface IResolverFactory<in TInfo>
    {
        ResolveDelegate<TContext> GetResolver<TContext>(TInfo info) 
            where TContext : IBuildContext;
    }
}
