using System.Reflection;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;

namespace Unity.Builder.Strategies
{
    public class DynamicMethodFieldsSetterStrategy : BuilderStrategy// CompiledStrategy<FieldInfo, object>
    {
        #region BuilderStrategy

        public override void PreBuildUp(ref BuilderContext context)
        {
            var dynamicBuildContext = (DynamicBuildPlanGenerationContext)context.Existing;

            var selector = context.GetPolicy<IFieldSelectorPolicy>(context.OriginalBuildKey.Type,
                context.OriginalBuildKey.Name);

        }

        #endregion
    }
}
