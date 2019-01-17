using System;
using System.Collections.Generic;
using System.Linq;
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
#if NETSTANDARD1_0
            return GetPropertiesHierarchical(type)
               .Where(p =>
               {
                   if (!p.CanWrite) return false;

                   var propertyMethod = p.GetSetMethod(true) ??
                                        p.GetGetMethod(true);

                   // Skip static properties and indexers. 
                   if (propertyMethod.IsStatic || p.GetIndexParameters().Length != 0)
                       return false;

                   return true;
               });
#else
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0);
#endif
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
                info.SetValue(context.Existing, context.Resolve(info, value));
                return context.Existing;
            };
        }

        #endregion


        #region Implementation

#if NETSTANDARD1_0
        public static IEnumerable<PropertyInfo> GetPropertiesHierarchical(Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<PropertyInfo>();
            }

            if (type == typeof(object))
            {
                return type.GetTypeInfo().DeclaredProperties;
            }

            return type.GetTypeInfo()
                       .DeclaredProperties
                       .Concat(GetPropertiesHierarchical(type.GetTypeInfo().BaseType));
        }
#endif

        #endregion
    }
}
