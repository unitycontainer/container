using System;

namespace Unity.Extension
{
    public delegate ResolveDelegate<TContext> PipelineFactory<TContext>(ref TContext context)
                where TContext : IResolveContext;
}

