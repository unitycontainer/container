

using System.Collections.Generic;
using Unity.Builder.Strategy;
using Unity.Policy;

namespace Unity.Strategy
{
    /// <summary>
    /// Represents a chain of responsibility for builder strategies.
    /// </summary>
    public interface IStrategyChain : IEnumerable<BuilderStrategy>, IBuildPlanPolicy
    {
    }
}
