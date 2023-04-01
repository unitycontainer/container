using Unity.Builder;

namespace Unity.Extension
{
    public delegate void BuilderStrategyDelegate<TContext>(ref TContext context)
        where TContext : IBuilderContext;
}
