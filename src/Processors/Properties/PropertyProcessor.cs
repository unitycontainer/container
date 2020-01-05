using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Resolution;

namespace Unity.Processors
{
    public class PropertyProcessor : MemberProcessor<PropertyInfo, object>
    {
        #region Overrides

        protected override Type MemberType(PropertyInfo info) => info.PropertyType;

        protected override IEnumerable<PropertyInfo> DeclaredMembers(Type type) => type.SupportedProperties();

        #endregion


        #region Expression 

        protected override Expression GetResolverExpression(PropertyInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<BuilderContext>(info);

            return Expression.Assign(
                Expression.Property(Expression.Convert(BuilderContext.ExistingExpression, info.DeclaringType), info),
                Expression.Convert(
                    Expression.Call(BuilderContext.ContextExpression,
                        BuilderContext.ResolvePropertyMethod,
                        Expression.Constant(info, typeof(PropertyInfo)),
                        Expression.Constant(attribute.Name, typeof(string)),
                        Expression.Constant(resolver, typeof(ResolveDelegate<BuilderContext>))),
                    info.PropertyType));
        }

        protected override Expression GetResolverExpression(PropertyInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<BuilderContext>? resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
            {
                return Expression.Assign(
                    Expression.Property(Expression.Convert(BuilderContext.ExistingExpression, info.DeclaringType), info),
                    Expression.Convert(
                        Expression.Call(BuilderContext.ContextExpression,
                            BuilderContext.OverridePropertyMethod,
                            Expression.Constant(info, typeof(PropertyInfo)),
                            Expression.Constant(attribute.Name, typeof(string)),
                            Expression.Constant(data, typeof(object))),
                        info.PropertyType));
            }
            else
            {
                return Expression.Assign(
                    Expression.Property(Expression.Convert(BuilderContext.ExistingExpression, info.DeclaringType), info),
                    Expression.Convert(
                        Expression.Call(BuilderContext.ContextExpression,
                            BuilderContext.ResolvePropertyMethod,
                            Expression.Constant(info, typeof(PropertyInfo)),
                            Expression.Constant(attribute.Name, typeof(string)),
                            Expression.Constant(resolver, typeof(ResolveDelegate<BuilderContext>))),
                        info.PropertyType));
            }
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(PropertyInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<BuilderContext>(info);

            return (ref BuilderContext context) =>
            {
                info.SetValue(context.Existing, context.Resolve(info, attribute.Name, resolver));
                return context.Existing;
            };
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(PropertyInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<BuilderContext>? resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
            {
                return (ref BuilderContext context) =>
                {
                    info.SetValue(context.Existing, context.Override(info, attribute.Name, data));
                    return context.Existing;
                };
            }
            else
            { 
                return (ref BuilderContext context) =>
                {
                    info.SetValue(context.Existing, context.Resolve(info, attribute.Name, resolver));
                    return context.Existing;
                };
            }
        }

        #endregion
    }
}
