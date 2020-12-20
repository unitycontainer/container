using System;

namespace Unity.Container
{
    public delegate T PipelineVisitor<T>(ref Pipeline_Builder<T> builder);
}
