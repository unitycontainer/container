using System;

namespace Unity.Extension
{
    public delegate ResolveDelegate<TContext> ResolverFactory<TContext>(Type type)
                where TContext : IResolveContext;
}

