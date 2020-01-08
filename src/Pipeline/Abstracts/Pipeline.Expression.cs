using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public abstract partial class Pipeline
    {
        public delegate ResolveDelegate<PipelineContext> PipelineBuilderDelegate(ref PipelineBuilder context);

        #region Fields

        private static readonly Type _contextRefType =
            typeof(PipelineBuilderDelegate).GetTypeInfo()
                                           .GetDeclaredMethod("Invoke")!
                                           .GetParameters()[0]
                                           .ParameterType;

        protected static readonly ParameterExpression ContextExpression =
            Expression.Parameter(_contextRefType, "context");

        public static readonly LabelTarget BuilderReturnTarget = Expression.Label(typeof(PipelineBuilderDelegate));

        #endregion


        #region Factory

        public static PipelineBuilderDelegate ExpressFactory(Pipeline[] pipelines)
        {
            ParameterExpression variable = Expression.Variable(typeof(PipelineBuilderDelegate));

            try
            {
                var result = pipelines.Where(SelectPre).ToArray();

                //var dd = result.SelectMany()

                var expressions = new List<Expression>();

                expressions.Add(Expression.Label(BuilderReturnTarget, PipelineContext.ExistingExpression));

                var lambda = Expression.Lambda<PipelineBuilderDelegate>(
                    Expression.Block(expressions), ContextExpression);

                return lambda.Compile();
            }
            catch (Exception ex)
            {
                return (ref PipelineBuilder context) => throw ex;
            }
        }

        #endregion


        #region Implementation
        
        private static bool SelectPre(Pipeline pipeline)
        {
            var method = pipeline.GetType().GetMethod("BuildExpression");

            if (typeof(Pipeline) == method.DeclaringType) return false;

            return true;
        }

        #endregion
    }
}
