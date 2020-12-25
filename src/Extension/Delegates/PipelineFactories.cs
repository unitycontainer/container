using System;

namespace Unity.Extension
{
    public delegate ResolveDelegate<TContext> PipelineFactory<TContext>(ref TContext context)
                where TContext : IBuilderContext;

    public delegate ResolveDelegate<TContext> FromTypeFactory<TContext>(Type type)
                where TContext : IResolveContext;
}

