using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Utility;

namespace Unity.Processors
{
    public class PropertiesProcessor : MemberBuildProcessor<PropertyInfo, object>
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

        
        #region Overrides

        public override IEnumerable<object> Select(ref BuilderContext context) =>
            base.Select(ref context).Distinct();

        protected override PropertyInfo[] DeclaredMembers(Type type)
        {
#if NETSTANDARD1_0
            return type.GetPropertiesHierarchical()
                       .Where(p =>
                       {
                           if (!p.CanWrite) return false;

                           var propertyMethod = p.GetSetMethod(true) ??
                                                p.GetGetMethod(true);

                           // Skip static properties and indexers. 
                           if (propertyMethod.IsStatic || p.GetIndexParameters().Length != 0)
                               return false;

                           return true;
                       })
                      .ToArray();
#else
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0)
                .ToArray();
#endif
        }

        protected override Type MemberType(PropertyInfo info) 
            => info.PropertyType;

        protected override Expression ResolveExpression(PropertyInfo property, string name, object resolver)
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
    }
}
