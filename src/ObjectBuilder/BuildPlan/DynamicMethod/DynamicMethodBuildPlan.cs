using System;
using Unity.Build;
using Unity.Builder;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan.DynamicMethod
{
    public class DynamicMethodBuildPlan : IBuildPlanPolicy, IResolverPolicy
    {
        private readonly Delegate _buildMethod;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buildMethod"></param>
        public DynamicMethodBuildPlan(Delegate buildMethod)
        {
            _buildMethod = buildMethod;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void BuildUp<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            context.Existing = ((BuildDelegate<TBuilderContext>)_buildMethod).Invoke(ref context);

        }

        public object Resolve<TContext>(ref TContext context) 
            where TContext : IBuildContext
        {
            return ((BuildDelegate<TContext>)_buildMethod).Invoke(ref context);
        }
    }
}
