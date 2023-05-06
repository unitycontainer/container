using Unity.Builder;
using Unity.Resolution;

namespace Unity.Extension
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate ResolveDelegate<TContext> PipelineFactory<TContext>(ref TContext context)
                where TContext : IBuilderContext;
}

