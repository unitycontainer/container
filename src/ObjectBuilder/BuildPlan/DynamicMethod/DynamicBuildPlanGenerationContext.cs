using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Build;
using Unity.Builder;
using Unity.Builder.Expressions;

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

        internal BuildDelegate<TBuilderContext> GetBuildMethod<TBuilderContext>()
            where TBuilderContext : IBuilderContext
        {
            var block = Expression.Block(
                _buildPlanExpressions.Concat(new[] { BuilderContextExpression<TBuilderContext>.Existing }));

            var lambda = Expression.Lambda<BuildDelegate<TBuilderContext>>(block,
                BuilderContextExpression<TBuilderContext>.Context);

            var planDelegate = lambda.Compile();

            return (ref TBuilderContext context) =>
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
