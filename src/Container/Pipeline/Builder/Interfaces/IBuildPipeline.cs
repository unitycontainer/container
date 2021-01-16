using System;

namespace Unity.Extension
{
    public interface IBuildPipeline<TContext> 
        where TContext : IBuilderContext
    {
        ResolveDelegate<TContext>? Build();
    }
}
