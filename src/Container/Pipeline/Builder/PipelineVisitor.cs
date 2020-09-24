using System;

namespace Unity.Container
{
    public delegate T PipelineVisitor<T>(ref PipelineBuilder<T> builder);
}
