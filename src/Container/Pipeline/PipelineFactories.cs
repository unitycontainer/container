using System;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public delegate ResolveDelegate<TContext> PipelineFactory<TContext>(Type type)
                where TContext : IResolveContext;

    public delegate ResolveDelegate<TContext> ContextualFactory<TContext>(ref TContext context)
                where TContext : IResolveContext;
}

