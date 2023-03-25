using Unity.Builder;

namespace Unity.Strategies
{
    /// <summary>
    /// Represents a strategy in the chain of responsibility.
    /// Strategies are required to support both BuildUp and TearDown.
    /// </summary>
    public abstract partial class BuilderStrategy
    {
        #region Analysis

        public virtual object? Analyze<TContext>(ref TContext context)
           where TContext : IBuilderContext => context.Contract.Type;

        #endregion
    }
}
