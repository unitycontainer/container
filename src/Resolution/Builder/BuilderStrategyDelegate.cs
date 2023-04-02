using Unity.Builder;

namespace Unity.Resolution
{
    public delegate void BuilderStrategyDelegate<TContext>(ref TContext context)
        where TContext : IBuilderContext;
}
