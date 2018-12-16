using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder.Expressions;
using Unity.Builder.Selection;
using Unity.Builder.Strategy;
using Unity.Injection;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan.DynamicMethod.Property
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that generates IL to resolve properties
    /// on an object being built.
    /// </summary>
    public class DynamicMethodPropertySetterStrategy : BuilderStrategy
    {
        #region BuilderStrategy

        /// <summary>
        /// Called during the chain of responsibility for a build operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override void PreBuildUp<TBuilderContext>(ref TBuilderContext context)
        {
            var dynamicBuildContext = (DynamicBuildPlanGenerationContext)context.Existing;

            var selector = context.Policies.GetPolicy<IPropertySelectorPolicy>(context.OriginalBuildKey.Type, 
                                                                               context.OriginalBuildKey.Name);

            foreach (var property in selector.SelectProperties(ref context))
            {
                ParameterExpression resolvedObjectParameter;

                switch (property)
                {
                    case SelectedProperty selectedProperty:
                        var propertyInfoExpression = Expression.Constant(selectedProperty.Property);
                        resolvedObjectParameter = Expression.Parameter(selectedProperty.Property.PropertyType);

                        dynamicBuildContext.AddToBuildPlan(
                            Expression.Block(
                                new[] { resolvedObjectParameter },
                                Expression.Assign(BuilderContextExpression<TBuilderContext>.CurrentOperation, propertyInfoExpression),
                                Expression.Assign(
                                    resolvedObjectParameter,
                                    BuilderContextExpression<TBuilderContext>.Resolve(selectedProperty.Property, context.OriginalBuildKey.Name, selectedProperty.Resolver)),
                                Expression.Call(
                                    Expression.Convert(
                                        BuilderContextExpression<TBuilderContext>.Existing,
                                        dynamicBuildContext.TypeToBuild),
                                    GetValidatedPropertySetter(selectedProperty.Property),
                                    resolvedObjectParameter)));
                        break;

                    case InjectionProperty injectionProperty:
                        var (info, value) = injectionProperty.Select(context.Type);
                        resolvedObjectParameter = Expression.Parameter(info.PropertyType);

                        dynamicBuildContext.AddToBuildPlan(
                            Expression.Block(
                                new[] { resolvedObjectParameter },
                                Expression.Assign(BuilderContextExpression<TBuilderContext>.CurrentOperation, Expression.Constant(info)),
                                Expression.Assign(
                                    resolvedObjectParameter,
                                    BuilderContextExpression<TBuilderContext>.Resolve(info, context.OriginalBuildKey.Name, value)),
                                Expression.Call(
                                    Expression.Convert(
                                        BuilderContextExpression<TBuilderContext>.Existing,
                                        dynamicBuildContext.TypeToBuild),
                                    GetValidatedPropertySetter(info),
                                    resolvedObjectParameter)));
                        break;

                    default:
                        throw new InvalidOperationException("Unknown type of property");
                }
            }

            dynamicBuildContext.AddToBuildPlan(Expression.Assign(BuilderContextExpression<TBuilderContext>.CurrentOperation, Expression.Constant(null)));
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
