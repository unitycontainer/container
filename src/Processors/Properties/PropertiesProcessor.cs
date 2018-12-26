using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Expressions;
using Unity.Injection;
using Unity.Policy;
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


        #region BuilderStrategy

        /// <inheritdoc />
        public override IEnumerable<Expression> GetEnumerator(ref BuilderContext context)
        {
            var selector = GetPolicy<ISelect<PropertyInfo>>(ref context, context.RegistrationType, context.RegistrationName);
            var properties = selector.Select(ref context);

            return GetEnumerator(context.Type, context.Name, context.Variable, properties);
        }

        #endregion


        #region Implementation

        private IEnumerable<Expression> GetEnumerator(Type type, string name, ParameterExpression variable, IEnumerable<object> properties)
        {
            foreach (var property in properties)
            {
                DependencyResolutionAttribute attribute;
                MemberExpression propExpr;

                switch (property)
                {
                    case PropertyInfo propertyInfo:
                        propExpr = Expression.Property(variable, propertyInfo);
                        attribute = (DependencyResolutionAttribute)propertyInfo.GetCustomAttribute(typeof(DependencyResolutionAttribute));

                        yield return propertyInfo.IsDefined(typeof(OptionalDependencyAttribute))
                            ? GetOptionalPropertyExpression(propExpr, propertyInfo, attribute?.Name ?? name)
                            : Expression.Assign(propExpr, BuilderContextExpression.Resolve(propertyInfo, attribute?.Name ?? name));
                        break;

                    case MemberInfoMember<PropertyInfo> injectionProperty:
                        var (info, value) = injectionProperty.FromType(type);
                        propExpr = Expression.Property(variable, info);
                        attribute = (DependencyResolutionAttribute)info.GetCustomAttribute(typeof(DependencyResolutionAttribute));
                        yield return attribute is OptionalDependencyAttribute
                            ? GetOptionalPropertyExpression(propExpr, info, attribute?.Name ?? name, value)
                            : Expression.Assign(propExpr, BuilderContextExpression.Resolve(info, attribute?.Name ?? name, value));
                        break;

                    default:
                        throw new InvalidOperationException("Unknown type of property");
                }
            }
        }

        private Expression GetOptionalPropertyExpression(MemberExpression propExpr, PropertyInfo info, string name, object value = null)
        {
            return Expression.TryCatch(
                        Expression.Assign(propExpr, BuilderContextExpression.Resolve(info, name, value)),
                    Expression.Catch(typeof(Exception),
                        Expression.Assign(propExpr, Expression.Constant(null, info.PropertyType))));
        }

        private static MethodInfo GetValidatedPropertySetter(PropertyInfo property)
        {
            // TODO: Check - Added a check for private to meet original expectations;
            var setter = property.GetSetMethod(true);
            if (setter == null || setter.IsPrivate)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    Constants.PropertyNotSettable, property.Name, property.DeclaringType?.FullName));
            }
            return setter;
        }

        #endregion

    }
}
