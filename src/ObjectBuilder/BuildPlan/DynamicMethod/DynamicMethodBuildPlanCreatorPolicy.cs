using System;
using Unity.Builder;
using Unity.Policy;
using Unity.Storage;

namespace Unity.ObjectBuilder.BuildPlan.DynamicMethod
{
    /// <summary>
    /// An <see cref="IBuildPlanCreatorPolicy"/> implementation
    /// that constructs a build plan via dynamic IL emission.
    /// </summary>
    public class DynamicMethodBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy
    {
        private BuilderStrategy[] _strategies;

        /// <summary>
        /// Construct a <see cref="DynamicMethodBuildPlanCreatorPolicy"/> that
        /// uses the given strategy chain to construct the build plan.
        /// </summary>
        /// <param name="strategies">The strategy chain.</param>
        public DynamicMethodBuildPlanCreatorPolicy(StagedStrategyChain<BuilderStrategy, BuilderStage> strategies)
        {
            strategies.Invalidated += (s, e) => _strategies = strategies.ToArray();
            _strategies = strategies.ToArray();
        }

        /// <summary>
        /// Construct a build plan.
        /// </summary>
        /// <param name="context">The current build context.</param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns>The created build plan.</returns>
        public IBuildPlanPolicy CreatePlan(ref BuilderContext context, Type type, string name)
        {
            var generatorContext = new DynamicBuildPlanGenerationContext(type);

            var planContext = new BuilderContext(context, generatorContext);

            _strategies.ExecutePlan(ref planContext);

            return new DynamicMethodBuildPlan(generatorContext.GetBuildMethod());
        }
    }
}
