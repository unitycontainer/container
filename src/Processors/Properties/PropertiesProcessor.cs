using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Storage;
using Unity.Utility;

namespace Unity.Processors
{
    public class PropertiesProcessor : BuildMemberProcessor<PropertyInfo, object>
    {
        #region Fields

        public static readonly MethodInfo ResolveProperty =
            typeof(BuilderContext).GetTypeInfo()
                .GetDeclaredMethods(nameof(BuilderContext.Resolve))
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return 2 <= parameters.Length &&
                        typeof(PropertyInfo) == parameters[0].ParameterType;
                });

        #endregion


        #region Constructors

        public PropertiesProcessor(IPolicySet policySet)
            : base(policySet)
        {
        }

        #endregion


        #region Overrides

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

        protected override Type MemberType(PropertyInfo info) 
            => info.PropertyType;

        protected override ResolveDelegate<BuilderContext> GetResolver(PropertyInfo info, object resolver)
        {
            return (ref BuilderContext context) =>
            {
                info.SetValue(context.Existing, context.Resolve(info, context.Name, resolver));
                return context.Existing;
            };
        }

        protected override Expression GetExpression(PropertyInfo property, string name, object resolver)
        {
            return Expression.Convert(
                Expression.Call(BuilderContextExpression.Context, ResolveProperty,
                    Expression.Constant(property, typeof(PropertyInfo)),
                    Expression.Constant(name, typeof(string)),
                    Expression.Constant(resolver, typeof(object))),
                property.PropertyType);
        }

        protected override MemberExpression CreateMemberExpression(PropertyInfo info) 
            => Expression.Property(Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType), info);

        #endregion


        #region Parameter Resolver Factories

        protected override ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue)
        {
            return (ref BuilderContext context) =>
            {
                ((PropertyInfo)info).SetValue(context.Existing, 
                    context.Resolve((PropertyInfo)info, ((DependencyResolutionAttribute)attribute).Name, resolver));

                return context.Existing;
            };
        }

        protected override ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue)
        {
            return (ref BuilderContext context) =>
            {
                try
                {
                    ((PropertyInfo)info).SetValue(context.Existing, 
                        context.Resolve((PropertyInfo)info, ((DependencyResolutionAttribute)attribute).Name, resolver));
                    return context.Existing;
                }
                catch
                {
                    ((PropertyInfo)info).SetValue(context.Existing, defaultValue);
                    return context.Existing;
                }
            };
        }

        #endregion


        #region Implementation

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

        #endregion
    }
}
