using System;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.ObjectBuilder.BuildPlan.DynamicMethod
{
    public class DynamicMethodBuildPlan : IBuildPlanPolicy, IResolve
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
        public void BuildUp(ref BuilderContext context)
        {
            context.Existing = ((ResolveDelegate<BuilderContext>)_buildMethod).Invoke(ref context);

        }

        public object Resolve<TContext>(ref TContext context) 
            where TContext : IResolveContext
        {
            return ((ResolveDelegate<TContext>)_buildMethod).Invoke(ref context);
        }
    }
}
