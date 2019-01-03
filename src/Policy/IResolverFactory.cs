using System;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Policy
{
    public delegate ResolveDelegate<TContext> ResolverFactory<TContext>(Type type) 
        where TContext : IResolveContext;

    public interface IResolverFactory
    {
        ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IResolveContext;
    }

    public interface IResolverFactory<TMemberInfo>
    {
        ResolveDelegate<TContext> GetResolver<TContext>(TMemberInfo info)
            where TContext : IResolveContext;
    }
}
