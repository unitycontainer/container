using Unity.Builder;
using Unity.Resolution;

namespace Unity.Extension
{
    public delegate ResolveDelegate<TContext> PipelineFactory<TContext>(ref TContext context)
                where TContext : IBuilderContext;
}

