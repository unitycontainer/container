using System;
using Unity.Resolution;

namespace Unity.Extension
{
    public interface IBuildPipeline<TContext> 
        where TContext : IBuilderContext
    {
        ResolveDelegate<TContext>? Build();
    }
}
