using Unity.Builder;

namespace Unity.Resolution
{
    public delegate ResolveDelegate<TContext> PipelineFactory<TContext>(ref TContext context)
                where TContext : IBuilderContext;
}

