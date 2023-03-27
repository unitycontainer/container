using Unity.Builder;

namespace Unity.Resolution
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

