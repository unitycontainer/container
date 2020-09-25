using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        #region Fields

        protected static readonly UnaryExpression ReThrowExpression =
            Expression.Rethrow(typeof(void));

        protected static readonly Expression GuidToObjectExpression =
            Expression.Convert(NewGuidExpression, typeof(object));

        protected static readonly MethodInfo AddMethodExpression =
            typeof(IDictionary).GetTypeInfo().GetDeclaredMethod(nameof(IDictionary.Add))!;

        protected static readonly UnaryExpression ConvertExpression =
            Expression.Convert(NewGuidExpression, typeof(object));

        protected static readonly MemberExpression ExceptionDataExpression =
            Expression.MakeMemberAccess(ExceptionVariableExpression, DataPropertyExpression);

        private const string _error = "Invalid 'ref' or 'out' parameter '{0}' ({1})";

        #endregion



        #region Expression

        protected virtual IEnumerable<Expression> ParameterExpressions(MethodBase info)
        {
            foreach (var parameter in info.GetParameters())
            {
                yield return GetResolverExpression(parameter);
            }
        }


        protected virtual IEnumerable<Expression> ParameterExpressions(MethodBase info, object[] injectors)
        {
            var parameters = info.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                yield return GetResolverExpression(parameters[i], injectors[i]);
            }
        }

        protected virtual Expression GetResolverExpression(ParameterInfo info)
        {
            throw new System.NotImplementedException();
            //var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
            //                                                                               ?? DependencyAttribute.Instance;
            //var resolver = attribute.GetResolver<PipelineContext>(info);

            //return Expression.Convert(
            //    Expression.Call(PipelineContext.ContextExpression,
            //                    PipelineContext.ResolveParameterMethod,
            //                    Expression.Constant(info, typeof(ParameterInfo)),
            //                    Expression.Constant(attribute.Name, typeof(string)),
            //                    Expression.Constant(resolver, typeof(ResolveDelegate<PipelineContext>))),
            //                info.ParameterType);
        }

        protected virtual Expression GetResolverExpression(ParameterInfo info, object? data)
        {
            throw new System.NotImplementedException();
            //var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
            //                                                                               ?? DependencyAttribute.Instance;
            //ResolveDelegate<PipelineContext>? resolver = PreProcessResolver(info, attribute, data);

            //if (null == resolver)
            //{
            //    return Expression.Convert(
            //        Expression.Call(PipelineContext.ContextExpression,
            //                        PipelineContext.OverrideParameterMethod,
            //                        Expression.Constant(info, typeof(ParameterInfo)),
            //                        Expression.Constant(attribute.Name, typeof(string)),
            //                        Expression.Constant(data, typeof(object))),
            //                    info.ParameterType);
            //}
            //else
            //{
            //    return Expression.Convert(
            //        Expression.Call(PipelineContext.ContextExpression,
            //                        PipelineContext.ResolveParameterMethod,
            //                        Expression.Constant(info, typeof(ParameterInfo)),
            //                        Expression.Constant(attribute.Name, typeof(string)),
            //                        Expression.Constant(resolver, typeof(ResolveDelegate<PipelineContext>))),
            //                    info.ParameterType);
            //}
        }

        #endregion
    }
}
