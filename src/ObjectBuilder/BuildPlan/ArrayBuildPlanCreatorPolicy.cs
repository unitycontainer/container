using System;
using Unity.Builder;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan
{
    public class ArrayBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy
    {
        public IBuildPlanPolicy CreatePlan(IBuilderContext context, NamedTypeBuildKey buildKey)
        {
            throw new NotImplementedException();
        }
    }
}
