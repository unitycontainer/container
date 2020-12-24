namespace Unity.Extension
{
    public delegate void PipelineVisitor<TContext>(ref TContext context)
        where TContext : IBuilderContext;
}

