using System.Collections.Generic;
using Unity.Builder;

namespace Unity.Storage
{
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
        void BuildUp<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext;

    }

    public static class BuildPlanPolicyExtensions
    {
        /// <summary>
        /// Execute this strategy chain against the given context,
        /// calling the Buildup methods on the strategies.
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="context">Context for the build process.</param>
        /// <returns>The build up object</returns>
        public static object ExecuteBuildUp<TBuilderContext>(this IStrategyChain policy, ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            policy.BuildUp(ref context);
            return context.Existing;
        }

    }
}
