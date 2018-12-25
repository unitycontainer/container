using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder.Expressions;
using Unity.Injection;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;
using Unity.Strategies;

namespace Unity.Builder.Strategies
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that generates expressions to resolve properties
    /// on an object being built.
    /// </summary>
    public class DynamicMethodPropertySetterStrategy : BuilderStrategy
    {
        #region BuilderStrategy

        /// <summary>
        /// Called during the chain of responsibility for a build operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override void PreBuildUp(ref BuilderContext context)
        {
            var dynamicBuildContext = (DynamicBuildPlanGenerationContext)context.Existing;

            var selector = GetPolicy<IPropertySelectorPolicy>(ref context, 
                context.RegistrationType, context.RegistrationName);

            foreach (var property in selector.SelectProperties(ref context))
            {
                ParameterExpression resolvedObjectParameter;

                switch (property)
                {
                    case PropertyInfo propertyInfo:
                        resolvedObjectParameter = Expression.Parameter(propertyInfo.PropertyType);

                        dynamicBuildContext.AddToBuildPlan(
                            Expression.Block(
                                new[] { resolvedObjectParameter },
                                Expression.Assign(
                                    resolvedObjectParameter,
                                    BuilderContextExpression.Resolve(propertyInfo, 
                                                                     context.RegistrationName,
                                                                     propertyInfo.GetResolver())),
                                Expression.Call(
                                    Expression.Convert(
                                        BuilderContextExpression.Existing,
                                        dynamicBuildContext.TypeToBuild),
                                    GetValidatedPropertySetter(propertyInfo),
                                    resolvedObjectParameter)));
                        break;

                    case MemberInfoMember<PropertyInfo> injectionProperty:
                        var (info, value) = injectionProperty.FromType(context.Type);
                        resolvedObjectParameter = Expression.Parameter(info.PropertyType);

                        dynamicBuildContext.AddToBuildPlan(
                            Expression.Block(
                                new[] { resolvedObjectParameter },
                                Expression.Assign(
                                    resolvedObjectParameter,
                                    BuilderContextExpression.Resolve(info, 
                                                                     context.RegistrationName, 
                                                                     value)),
                                Expression.Call(
                                    Expression.Convert(
                                        BuilderContextExpression.Existing,
                                        dynamicBuildContext.TypeToBuild),
                                    GetValidatedPropertySetter(info),
                                    resolvedObjectParameter)));
                        break;

                    default:
                        throw new InvalidOperationException("Unknown type of property");
                }
            }
        }

        #endregion


        #region Implementation

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
