using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.ObjectBuilder.Policies;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan.DynamicMethod
{
    public class DynamicMethodTypeMappingStrategy : BuilderStrategy
    {
        public override void PreBuildUp(IBuilderContext context)
        {
            var plan = context.PersistentPolicies.Get<IBuildPlanPolicy>(context.BuildKey, out _);
            if (plan == null || plan is OverriddenBuildPlanMarkerPolicy) return;

            context.Existing = plan;
            context.BuildComplete = true;
        }
    }
}
