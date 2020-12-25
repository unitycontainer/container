using System.Linq;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Unity.Benchmarks
{
    /// <summary>
    /// An extension to install custom strategy that disables
    /// saving of created build plan
    /// </summary>
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

    /// <summary>
    /// Unity v5 uses <see cref="BuildPlanStrategy"/> to create and save
    /// a build plan for each created type
    /// </summary>
    public class PipelineSpyStrategy : BuildPlanStrategy
    {
        // Override default implementation
        public override void PreBuildUp(IBuilderContext context)
        {
            IPolicyList buildPlanLocation;

            var plan = context.Policies.Get<IBuildPlanPolicy>(context.BuildKey, out buildPlanLocation);
            if (plan == null)
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
