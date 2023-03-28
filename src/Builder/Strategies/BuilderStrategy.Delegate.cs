using Unity.Builder;

namespace Unity.Strategies
{
    public delegate void BuilderStrategyDelegate<TContext>(ref TContext context)
        where TContext : IBuilderContext;
}
