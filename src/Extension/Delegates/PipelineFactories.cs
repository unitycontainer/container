using System;
using Unity.Extension;

namespace Unity.Extension
{
    public delegate ResolveDelegate<TContext> PipelineFactory<TContext>(Type type)
                where TContext : IResolveContext;

    public delegate ResolveDelegate<TContext> ContextualFactory<TContext>(ref TContext context)
                where TContext : IResolveContext;
}

