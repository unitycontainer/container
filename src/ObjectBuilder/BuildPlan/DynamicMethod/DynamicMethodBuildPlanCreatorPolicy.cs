using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly StagedStrategyChain<BuilderStrategy, BuilderStage> _strategies;

        /// <summary>
        /// Construct a <see cref="DynamicMethodBuildPlanCreatorPolicy"/> that
        /// uses the given strategy chain to construct the build plan.
        /// </summary>
        /// <param name="strategies">The strategy chain.</param>
        public DynamicMethodBuildPlanCreatorPolicy(StagedStrategyChain<BuilderStrategy, BuilderStage> strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Construct a build plan.
        /// </summary>
        /// <param name="context">The current build context.</param>
        /// <param name="buildKey">The current build key.</param>
        /// <returns>The created build plan.</returns>
        public IBuildPlanPolicy CreatePlan<TBuilderContext>(ref TBuilderContext context, INamedType buildKey)
            where TBuilderContext : IBuilderContext
        {
            var generatorContext = new DynamicBuildPlanGenerationContext(buildKey.Type);

            var planContext = new BuilderContext(context, _strategies, generatorContext);

            var i = -1;
            var chain = _strategies.ToArray();

            try
            {
                while (!context.BuildComplete && ++i < chain.Length)
                {
                    chain[i].PreBuildUp(ref planContext);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref planContext);
                }
            }
            catch (Exception)
            {
                context.RequiresRecovery?.Recover();
                throw;
            }

            return new DynamicMethodBuildPlan(generatorContext.GetBuildMethod<TBuilderContext>());
        }
    }
}
