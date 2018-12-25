using System.Collections.Generic;
using Unity.Builder;
using Unity.Strategies;

namespace Unity.Storage
{
    // TODO: Remove

    /// <summary>
    /// Represents a chain of responsibility for builder strategies.
    /// </summary>
    public interface IStrategyChain : IEnumerable<BuilderStrategy>
    {
        /// <summary>
        /// Creates an instance of this build plan's type, or fills
        /// in the existing type if passed in.
        /// </summary>
        /// <param name="context">Context used to build up the object.</param>
        void BuildUp<TBuilderContext>(ref BuilderContext context);

    }
}
