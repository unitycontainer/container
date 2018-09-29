using System;
using Unity.Builder;
using Unity.Delegates;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan.DynamicMethod
{

    /// <summary>
    /// 
    /// </summary>
    public class DynamicMethodBuildPlan : IBuildPlanPolicy
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
            ((ResolveDelegate<TBuilderContext>)_buildMethod).Invoke(ref context);
        }
    }
}
