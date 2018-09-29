using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Delegates;
using Unity.Expressions;
using Unity.Policy;
using Unity.Builder.Selection;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="parameterType"></param>
        /// <param name="setOperationExpression"></param>
        /// <returns></returns>
        public Expression CreateParameterExpression<TBuilderContext>(IResolverPolicy resolver, Type parameterType, Expression setOperationExpression)
            where TBuilderContext : IBuilderContext
        {
            // The intent of this is to create a parameter resolving expression block. The following
            // pseudo code will hopefully make it clearer as to what we're trying to accomplish (of course actual code
            // trumps comments):
            //  object priorOperation = context.CurrentOperation;
            //  SetCurrentOperation
            //  var resolver = GetResolver([context], [paramType], [key])
            //  var dependencyResult = resolver.ResolveDependency([context]);   
            //  context.CurrentOperation = priorOperation;
            //  dependencyResult ; // return item from Block

            var savedOperationExpression = Expression.Parameter(typeof(object));
            var resolvedObjectExpression = Expression.Parameter(parameterType);
            return
                Expression.Block(
                    new[] { savedOperationExpression, resolvedObjectExpression },
                    Expression.Assign(savedOperationExpression, BuilderContextExpression<TBuilderContext>.CurrentOperation),
                    setOperationExpression,
                    Expression.Assign(resolvedObjectExpression, GetResolveDependencyExpression<TBuilderContext>(parameterType, resolver)),
                    Expression.Assign(BuilderContextExpression<TBuilderContext>.CurrentOperation, savedOperationExpression),
                    resolvedObjectExpression
                    );
        }

        internal Expression GetResolveDependencyExpression<TBuilderContext>(Type dependencyType, IResolverPolicy resolver)
            where TBuilderContext : IBuilderContext
        {
            var resolveDependencyMethod =
                typeof(IResolverPolicy).GetTypeInfo().GetDeclaredMethod(nameof(IResolverPolicy.Resolve))
                    .MakeGenericMethod(typeof(TBuilderContext));

            var getResolverMethod =
                    typeof(DynamicBuildPlanGenerationContext).GetTypeInfo()
                        .GetDeclaredMethod(nameof(GetResolver))
                        .MakeGenericMethod(typeof(TBuilderContext));

            return Expression.Convert(
                Expression.Call(
                    Expression.Call(null,
                        getResolverMethod,
                        BuilderContextExpression<TBuilderContext>.Context,
                        Expression.Constant(dependencyType, typeof(Type)),
                        Expression.Constant(resolver, typeof(IResolverPolicy))),
                    resolveDependencyMethod,
                    BuilderContextExpression<TBuilderContext>.Context),
                dependencyType);
        }

        internal ResolveDelegate<TBuilderContext> GetBuildMethod<TBuilderContext>()
            where TBuilderContext : IBuilderContext
        {
            var block = Expression.Block(
                _buildPlanExpressions.Concat(new[] { BuilderContextExpression<TBuilderContext>.Existing }));

            var lambda = Expression.Lambda<ResolveDelegate<TBuilderContext>>(block,
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

        /// <summary>
        /// Helper method used by generated IL to look up a dependency resolver based on the given key.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="dependencyType">Type of the dependency being resolved.</param>
        /// <param name="resolver">The configured resolver.</param>
        /// <returns>The found dependency resolver.</returns>
        public static IResolverPolicy GetResolver<TBuilderContext>(ref TBuilderContext context, Type dependencyType, IResolverPolicy resolver)
            where TBuilderContext : IBuilderContext
        {
            var overridden = context.GetOverriddenResolver(dependencyType);
            return overridden ?? resolver;
        }
    }
}
