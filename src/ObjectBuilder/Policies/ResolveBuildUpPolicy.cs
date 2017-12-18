using Unity.Builder;
using Unity.Policy;

namespace Unity.ObjectBuilder.Policies
{
    public class ResolveBuildUpPolicy : IBuildPlanPolicy
    {
        public void BuildUp(IBuilderContext context)
        {
            context.Existing = context.NewBuildUp(context.BuildKey);
            context.BuildComplete = null != context.Existing;
        }
    }
}
