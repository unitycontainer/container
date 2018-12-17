using System.Reflection;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;

namespace Unity.Builder.Strategies
{
    public class DynamicMethodFieldsSetterStrategy : BuilderStrategy// CompiledStrategy<FieldInfo, object>
    {
        #region BuilderStrategy

        public override void PreBuildUp<TBuilderContext>(ref TBuilderContext context)
        {
            var dynamicBuildContext = (DynamicBuildPlanGenerationContext)context.Existing;

            var selector = context.Policies.GetPolicy<IFieldSelectorPolicy>(context.OriginalBuildKey.Type,
                context.OriginalBuildKey.Name);

        }

        #endregion
    }
}
