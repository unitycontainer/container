using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Resolution;

namespace Unity
{
    public class FieldPipeline : MemberPipeline<FieldInfo, object>
    {
        #region Constructors

        public FieldPipeline(UnityContainer container)
            : base(container)
        {
        }

        #endregion


        #region Overrides
        
        protected override Type MemberType(FieldInfo info) => info.FieldType;

        protected override IEnumerable<FieldInfo> DeclaredMembers(Type type) => type.SupportedFields();

        #endregion


        #region Expression 

        protected override Expression GetResolverExpression(FieldInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<PipelineContext>(info);

            return Expression.Assign(
                Expression.Field(Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType), info),
                Expression.Convert(
                    Expression.Call(PipelineContext.ContextExpression,
                        PipelineContext.ResolveFieldMethod,
                        Expression.Constant(info, typeof(FieldInfo)),
                        Expression.Constant(attribute.Name, typeof(string)),
                        Expression.Constant(resolver, typeof(ResolveDelegate<PipelineContext>))),
                    info.FieldType));
        }

        protected override Expression GetResolverExpression(FieldInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<PipelineContext>? resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
            {
                return Expression.Assign(
                    Expression.Field(Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType), info),
                    Expression.Convert(
                        Expression.Call(PipelineContext.ContextExpression,
                            PipelineContext.OverrideFieldMethod,
                            Expression.Constant(info, typeof(FieldInfo)),
                            Expression.Constant(attribute.Name, typeof(string)),
                            Expression.Constant(data, typeof(object))),
                        info.FieldType));
            }
            else
            {
                return Expression.Assign(
                    Expression.Field(Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType), info),
                    Expression.Convert(
                        Expression.Call(PipelineContext.ContextExpression,
                            PipelineContext.ResolveFieldMethod,
                            Expression.Constant(info, typeof(FieldInfo)),
                            Expression.Constant(attribute.Name, typeof(string)),
                            Expression.Constant(resolver, typeof(ResolveDelegate<PipelineContext>))),
                        info.FieldType));
            }
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(FieldInfo info)
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

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(FieldInfo info, object? data)
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
