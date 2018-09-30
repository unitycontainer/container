using System.Collections.Generic;
using Unity.Policy;

namespace Unity.Builder.Strategy
{
    /// <summary>
    /// Represents a chain of responsibility for builder strategies.
    /// </summary>
    public interface IStrategyChain : IEnumerable<BuilderStrategy>, IBuildPlanPolicy
    {
    }
}
