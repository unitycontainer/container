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
#if NET40
            if (parameter.DefaultValue is DBNull)
#else
            if (!parameter.HasDefaultValue)
#endif
            {
                return (ref PipelineContext context) =>
                {
                    try
                    {
                        return context.Resolve(parameter, resolver);
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add(Guid.NewGuid(), parameter);
                        throw;
                    }
                };
            }
            else
            {
                // Check if has default value
#if NET40
                var defaultValue = !(parameter.DefaultValue is DBNull) ? parameter.DefaultValue : null;
#else
                var defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;
#endif
                return (ref PipelineContext context) =>
                {
                    try
                    {
                        return context.Resolve(parameter, resolver);
                    }
                    catch
                    {
                        return defaultValue;
                    }
                };
            }
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
