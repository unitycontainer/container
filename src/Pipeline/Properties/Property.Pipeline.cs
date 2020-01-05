using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity
{
    public class PropertyPipeline : MemberPipeline<PropertyInfo, object>
    {
        #region Constructors

        public PropertyPipeline(UnityContainer container)
            : base(container)
        {
        }

        #endregion


        #region Overrides

        protected override Type MemberType(PropertyInfo info) => info.PropertyType;

        protected override IEnumerable<PropertyInfo> DeclaredMembers(Type type) => UnityDefaults.SupportedProperties(type);

        #endregion


        #region Expression 

        protected override Expression GetResolverExpression(PropertyInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<PipelineContext>(info);

            return Expression.Assign(
                Expression.Property(Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType), info),
                Expression.Convert(
                    Expression.Call(PipelineContext.ContextExpression,
                        PipelineContext.ResolvePropertyMethod,
                        Expression.Constant(info, typeof(PropertyInfo)),
                        Expression.Constant(attribute.Name, typeof(string)),
                        Expression.Constant(resolver, typeof(ResolveDelegate<PipelineContext>))),
                    info.PropertyType));
        }

        protected override Expression GetResolverExpression(PropertyInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<PipelineContext>? resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
            {
                return Expression.Assign(
                    Expression.Property(Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType), info),
                    Expression.Convert(
                        Expression.Call(PipelineContext.ContextExpression,
                            PipelineContext.OverridePropertyMethod,
                            Expression.Constant(info, typeof(PropertyInfo)),
                            Expression.Constant(attribute.Name, typeof(string)),
                            Expression.Constant(data, typeof(object))),
                        info.PropertyType));
            }
            else
            {
                return Expression.Assign(
                    Expression.Property(Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType), info),
                    Expression.Convert(
                        Expression.Call(PipelineContext.ContextExpression,
                            PipelineContext.ResolvePropertyMethod,
                            Expression.Constant(info, typeof(PropertyInfo)),
                            Expression.Constant(attribute.Name, typeof(string)),
                            Expression.Constant(resolver, typeof(ResolveDelegate<PipelineContext>))),
                        info.PropertyType));
            }
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(PropertyInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<PipelineContext>(info);

            return (ref PipelineContext context) =>
            {
                info.SetValue(context.Existing, context.Resolve(info, attribute.Name, resolver));
                return context.Existing;
            };
        }

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(PropertyInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<PipelineContext>? resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
            {
                return (ref PipelineContext context) =>
                {
                    info.SetValue(context.Existing, context.Override(info, attribute.Name, data));
                    return context.Existing;
                };
            }
            else
            {
                return (ref PipelineContext context) =>
                {
                    info.SetValue(context.Existing, context.Resolve(info, attribute.Name, resolver));
                    return context.Existing;
                };
            }
        }

        #endregion
    }
}
