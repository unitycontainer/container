using Unity.Resolution;

namespace Unity.Container
{
    public delegate ResolveDelegate<PipelineContext> PipelineFactory<T>(ref T data);
}

