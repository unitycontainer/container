using Unity.Resolution;

namespace Unity.Container
{
    public delegate ResolveDelegate<PipelineContext> PipelineFactory(ref PipelineContext context);
}

