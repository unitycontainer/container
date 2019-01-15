using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public class FieldProcessor : MemberProcessor<FieldInfo, object>
    {
        #region Constructors

        public FieldProcessor(IPolicySet policySet)
            : base(policySet)
        {
        }

        #endregion


        #region Overrides

        protected override IEnumerable<FieldInfo> DeclaredMembers(Type type)
        {
#if NETSTANDARD1_0
            return GetFieldsHierarchical(type).Where(f => !f.IsInitOnly && !f.IsStatic);
#else
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                       .Where(f => !f.IsInitOnly && !f.IsStatic);
#endif
        }

        protected override Type MemberType(FieldInfo info) => info.FieldType;

        #endregion


        #region Expression 

        protected override Expression GetResolverExpression(FieldInfo info, object resolver)
        {
            return Expression.Assign(
                Expression.Field(Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType), info),
                Expression.Convert(
                    Expression.Call(BuilderContextExpression.Context,
                        BuilderContextExpression.ResolveFieldMethod,
                        Expression.Constant(info, typeof(FieldInfo)),
                        Expression.Constant(PreProcessResolver(info, resolver), typeof(object))),
                    info.FieldType));
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(FieldInfo info, object resolver)
        {
            var value = PreProcessResolver(info, resolver);
            return (ref BuilderContext context) =>
            {
                info.SetValue(context.Existing, context.Resolve(info, value));
                return context.Existing;
            };
        }

        #endregion


        #region Implementation
#if NETSTANDARD1_0

        public static IEnumerable<FieldInfo> GetFieldsHierarchical(Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<FieldInfo>();
            }

            if (type == typeof(object))
            {
                return type.GetTypeInfo().DeclaredFields;
            }

            return type.GetTypeInfo()
                .DeclaredFields
                .Concat(GetFieldsHierarchical(type.GetTypeInfo().BaseType));
        }
#endif
        #endregion
    }
}
