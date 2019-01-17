using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.ObjectBuilder.BuildPlan.DynamicMethod
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicBuildPlanGenerationContext
    {
        private readonly Queue<Expression> _buildPlanExpressions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeToBuild"></param>
        public DynamicBuildPlanGenerationContext(Type typeToBuild)
        {
            TypeToBuild = typeToBuild;
            _buildPlanExpressions = new Queue<Expression>();
        }

        /// <summary>
        /// The type that is to be built with the dynamic build plan.
        /// </summary>
        public Type TypeToBuild { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        public void AddToBuildPlan(Expression expression)
        {
            _buildPlanExpressions.Enqueue(expression);
        }

        internal ResolveDelegate<BuilderContext> GetBuildMethod()
        {
            var block = Expression.Block(
                _buildPlanExpressions.Concat(new[] { BuilderContextExpression.Existing }));

            var lambda = Expression.Lambda<ResolveDelegate<BuilderContext>>(block,
                BuilderContextExpression.Context);

            var planDelegate = lambda.Compile();

            return (ref BuilderContext context) =>
            {
                try
                {
                    context.Existing = planDelegate(ref context);
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException != null) throw e.InnerException;
                    throw;
                }

                return context.Existing;
            };
        }
    }
}
