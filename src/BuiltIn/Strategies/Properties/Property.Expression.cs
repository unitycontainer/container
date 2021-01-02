using System.Linq.Expressions;
using System.Reflection;

namespace Unity.Container
{
    public partial class PropertyStrategy
    {
        protected override Expression GetResolverExpression(PropertyInfo info)
        {
            throw new System.NotImplementedException();

            //var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
            //                                                                               ?? DependencyAttribute.Instance;
            //var resolver = attribute.GetResolver<PipelineContext>(info);

            //return Expression.Assign(
            //    Expression.Property(Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType), info),
            //    Expression.Convert(
            //        Expression.Call(PipelineContext.ContextExpression,
            //            PipelineContext.ResolvePropertyMethod,
            //            Expression.Constant(info, typeof(PropertyInfo)),
            //            Expression.Constant(attribute.Name, typeof(string)),
            //            Expression.Constant(resolver, typeof(ResolveDelegate<PipelineContext>))),
            //        info.PropertyType));
        }

        protected override Expression GetResolverExpression(PropertyInfo info, object? data)
        {
            throw new System.NotImplementedException();

            //var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
            //                                                                               ?? DependencyAttribute.Instance;
            //ResolveDelegate<PipelineContext>? resolver = PreProcessResolver(info, attribute, data);

            //if (null == resolver)
            //{
            //    return Expression.Assign(
            //        Expression.Property(Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType), info),
            //        Expression.Convert(
            //            Expression.Call(PipelineContext.ContextExpression,
            //                PipelineContext.OverridePropertyMethod,
            //                Expression.Constant(info, typeof(PropertyInfo)),
            //                Expression.Constant(attribute.Name, typeof(string)),
            //                Expression.Constant(data, typeof(object))),
            //            info.PropertyType));
            //}
            //else
            //{
            //    return Expression.Assign(
            //        Expression.Property(Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType), info),
            //        Expression.Convert(
            //            Expression.Call(PipelineContext.ContextExpression,
            //                PipelineContext.ResolvePropertyMethod,
            //                Expression.Constant(info, typeof(PropertyInfo)),
            //                Expression.Constant(attribute.Name, typeof(string)),
            //                Expression.Constant(resolver, typeof(ResolveDelegate<PipelineContext>))),
            //            info.PropertyType));
            //}
        }
    }
}
