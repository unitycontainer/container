using System;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity
{
    public partial class ParametersDiagnosticProcessor : ParametersProcessor
    {

        protected override ResolveDelegate<PipelineContext> ParameterResolverFactory(ParameterInfo parameter, object resolver)
        {
            var pipeline = base.ParameterResolverFactory(parameter, resolver);

            return (ref PipelineContext context) =>
            {
                try
                {
                    // TODO: Add validation

                    return pipeline(ref context);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), parameter);
                    throw;
                }
            };
        }

        protected override Expression ParameterExpressionFactory(ParameterExpression expression, ParameterInfo parameter, object resolver)
        {
            var tryBlock = base.ParameterExpressionFactory(expression, parameter, resolver);
            var catchBlock = Expression.Block(tryBlock.Type,
                    Expression.Call(ExceptionDataExpr, AddMethodInfo,
                        Expression.Convert(CallNewGuidExpr, typeof(object)),
                        Expression.Constant(parameter, typeof(object))),
                Expression.Rethrow(tryBlock.Type));

            return Expression.TryCatch(tryBlock,
                   Expression.Catch(ExceptionExpr, catchBlock));
        }
    }
}
