// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Builder;
using Unity.Policy;
using Unity.Strategy;

namespace Unity.ObjectBuilder.BuildPlan.DynamicMethod
{
    /// <summary>
    /// An <see cref="IBuildPlanCreatorPolicy"/> implementation
    /// that constructs a build plan via dynamic IL emission.
    /// </summary>
    public class DynamicMethodBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy
    {
        private readonly IStagedStrategyChain _strategies;

        /// <summary>
        /// Construct a <see cref="DynamicMethodBuildPlanCreatorPolicy"/> that
        /// uses the given strategy chain to construct the build plan.
        /// </summary>
        /// <param name="strategies">The strategy chain.</param>
        public DynamicMethodBuildPlanCreatorPolicy(IStagedStrategyChain strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Construct a build plan.
        /// </summary>
        /// <param name="context">The current build context.</param>
        /// <param name="buildKey">The current build key.</param>
        /// <returns>The created build plan.</returns>
        public IBuildPlanPolicy CreatePlan(IBuilderContext context, NamedTypeBuildKey buildKey)
        {
            var generatorContext =
                new DynamicBuildPlanGenerationContext((buildKey ?? throw new ArgumentNullException(nameof(buildKey))).Type);

            IBuilderContext planContext = GetContext((context ?? throw new ArgumentNullException(nameof(context))), buildKey, generatorContext);

            planContext.Strategies.ExecuteBuildUp(planContext);

            return new DynamicMethodBuildPlan(generatorContext.GetBuildMethod());
        }

        private IBuilderContext GetContext(IBuilderContext originalContext, NamedTypeBuildKey buildKey, DynamicBuildPlanGenerationContext generatorContext)
        {
            return new BuilderContext(originalContext.Container,
                                      _strategies.MakeStrategyChain(),
                                      originalContext.Lifetime,
                                      originalContext.PersistentPolicies,
                                      originalContext.Policies,
                                      buildKey,
                                      generatorContext);
        }
    }
}
