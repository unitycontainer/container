using System;

namespace Unity.Extension
{
    public interface IBuildPipeline<TContext> 
        where TContext : IBuilderContext
    {
        BuilderStrategy Strategy { get; }

        ResolveDelegate<TContext>? Build();
    }
}
