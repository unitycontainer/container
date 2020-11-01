using System;
using Unity.Resolution;

namespace Unity.Container
{
    public delegate ResolveDelegate<PipelineContext> PipelineFactory(Type type);
}

