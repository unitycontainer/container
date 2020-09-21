using System;

namespace Unity.Container
{
    public delegate void PipelineVisitor<T>(ref PipelineBuilder<T> builder);
}
