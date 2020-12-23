using BenchmarkDotNet.Attributes;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity.Lifetime;
using Unity;
#endif

namespace Unity.Benchmarks
{
    public class PipelineSpyExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            // Unity v4 did not offer any way of replacing built in strategies
            // The only way to do it is to clear and repopulate

            // Get an array of all strategies.
            var strategies = Context.Strategies.MakeStrategyChain()
                                               .ToArray();
            // Clear the chain
            Context.Strategies.Clear();

            // Repopulate main strategy chain, replacing the last one
            // Main strategy chain
            Context.Strategies.Add(strategies[0], UnityBuildStage.TypeMapping);
            Context.Strategies.Add(strategies[1], UnityBuildStage.Lifetime);
            Context.Strategies.Add(strategies[2], UnityBuildStage.Lifetime);
            Context.Strategies.Add(strategies[3], UnityBuildStage.Creation);

            Context.Strategies.Add(new PipelineSpyStrategy(), UnityBuildStage.Creation);
        }
    }

    public class PipelineSpyStrategy : BuildPlanStrategy
    {
        /// <summary>
        /// Override default build plan with this one
        /// </summary>
        public override void PreBuildUp(IBuilderContext context)
        {
            IPolicyList buildPlanLocation;

            var plan = context.Policies.Get<IBuildPlanPolicy>(context.BuildKey, out buildPlanLocation);
            if (plan == null /*|| plan is OverriddenBuildPlanMarkerPolicy*/)
            {
                IPolicyList creatorLocation;

                var planCreator = context.Policies.Get<IBuildPlanCreatorPolicy>(context.BuildKey, out creatorLocation);
                if (planCreator != null)
                {
                    plan = planCreator.CreatePlan(context, context.BuildKey);
                    // Disable pipeline save so it builds it every time
                    //(buildPlanLocation ?? creatorLocation).Set(plan, context.BuildKey);
                }
            }
            if (plan != null)
            {
                plan.BuildUp(context);
            }
        }
    }
}
