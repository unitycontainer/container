using System;

namespace Unity.Extension
{
    public delegate ResolveDelegate<TContext> PipelineFactory<TContext>(Type type)
                where TContext : IResolveContext;

    public delegate ResolveDelegate<TContext> ContextualFactory<TContext>(ref TContext context)
                where TContext : IResolveContext;
}

