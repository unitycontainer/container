using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Processors
{
    public class PropertyProcessor : MemberProcessor<PropertyInfo, object>
    {
        #region Constructors

        public PropertyProcessor(IPolicySet policySet)
            : base(policySet)
        {
        }

        #endregion


        #region Overrides

        protected override Type MemberType(PropertyInfo info) => info.PropertyType;

        protected override IEnumerable<PropertyInfo> DeclaredMembers(Type type)
        {
            foreach (var member in type.GetDeclaredProperties())
            {
                if (!member.CanWrite || 0 != member.GetIndexParameters().Length)
                    continue;

                var setter = member.GetSetMethod(true);
                if (setter.IsPrivate || setter.IsFamily)
                    continue;

                yield return member;
            }
        }

        #endregion


        #region Expression 

        protected override Expression GetResolverExpression(PropertyInfo info, object resolver)
        {
            return Expression.Assign(
                Expression.Property(Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType), info),
                Expression.Convert(
                    Expression.Call(BuilderContextExpression.Context,
                        BuilderContextExpression.ResolvePropertyMethod,
                        Expression.Constant(info, typeof(PropertyInfo)),
                        Expression.Constant(PreProcessResolver(info, resolver), typeof(object))),
                    info.PropertyType));
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(PropertyInfo info, object resolver)
        {
            var value = PreProcessResolver(info, resolver);
            return (ref BuilderContext context) =>
            {
#if NET40
                info.SetValue(context.Existing, context.Resolve(info, value), null);
#else
                info.SetValue(context.Existing, context.Resolve(info, value));
#endif
                return context.Existing;
            };
        }

        #endregion
    }
}
