using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Expressions;
using Unity.Utility;

namespace Unity.Processors
{
    public class PropertiesProcessor : MemberBuildProcessor<PropertyInfo, object>
    {
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

        #endregion


        #region Implementation

        protected override Type MemberType(PropertyInfo info) 
            => info.PropertyType;

        protected override Expression ResolveExpression(PropertyInfo member, string name, object resolver) 
            => BuilderContextExpression.Resolve(member, name, resolver);

        protected override MemberExpression CreateMemberExpression(ParameterExpression variable, PropertyInfo info) 
            => Expression.Property(variable, info);

        #endregion

    }
}
