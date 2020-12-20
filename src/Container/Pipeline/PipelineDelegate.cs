using Unity.Extension;

namespace Unity.Container
{
    public delegate void PipelineDelegate<TContext>(ref TContext context)
        where TContext : IBuilderContext;
}

