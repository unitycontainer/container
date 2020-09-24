using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class FieldProcessor
    {
        protected override Expression GetResolverExpression(FieldInfo info)
        {
            throw new System.NotImplementedException();
            //var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
            //                                                                               ?? DependencyAttribute.Instance;
            //var resolver = attribute.GetResolver<PipelineContext>(info);

            //return Expression.Assign(
            //    Expression.Field(Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType), info),
            //    Expression.Convert(
            //        Expression.Call(PipelineContext.ContextExpression,
            //            PipelineContext.ResolveFieldMethod,
            //            Expression.Constant(info, typeof(FieldInfo)),
            //            Expression.Constant(attribute.Name, typeof(string)),
            //            Expression.Constant(resolver, typeof(ResolveDelegate<PipelineContext>))),
            //        info.FieldType));
        }

        protected override Expression GetResolverExpression(FieldInfo info, object? data)
        {
            throw new System.NotImplementedException();
            //var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
            //                                                                               ?? DependencyAttribute.Instance;
            //ResolveDelegate<PipelineContext>? resolver = PreProcessResolver(info, attribute, data);

            //if (null == resolver)
            //{
            //    return Expression.Assign(
            //        Expression.Field(Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType), info),
            //        Expression.Convert(
            //            Expression.Call(PipelineContext.ContextExpression,
            //                PipelineContext.OverrideFieldMethod,
            //                Expression.Constant(info, typeof(FieldInfo)),
            //                Expression.Constant(attribute.Name, typeof(string)),
            //                Expression.Constant(data, typeof(object))),
            //            info.FieldType));
            //}
            //else
            //{
            //    return Expression.Assign(
            //        Expression.Field(Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType), info),
            //        Expression.Convert(
            //            Expression.Call(PipelineContext.ContextExpression,
            //                PipelineContext.ResolveFieldMethod,
            //                Expression.Constant(info, typeof(FieldInfo)),
            //                Expression.Constant(attribute.Name, typeof(string)),
            //                Expression.Constant(resolver, typeof(ResolveDelegate<PipelineContext>))),
            //            info.FieldType));
            //}
        }
    }
}
