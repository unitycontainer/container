using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder.Strategy;
using Unity.Expressions;
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
            var typeExpression = Expression.Constant(context.Type);
            var dynamicBuildContext = (DynamicBuildPlanGenerationContext)context.Existing;
            var selector = context.Policies.GetPolicy<IPropertySelectorPolicy>(context.OriginalBuildKey.Type, context.OriginalBuildKey.Name);

            foreach (var selectedProperty in selector.SelectProperties(ref context))
            {
                var propertyInfoExpression = Expression.Constant(selectedProperty.Property);
                var resolvedObjectParameter = Expression.Parameter(selectedProperty.Property.PropertyType);

                dynamicBuildContext.AddToBuildPlan(
                    Expression.Block(
                        new[] { resolvedObjectParameter },
                        Expression.Assign(BuilderContextExpression<TBuilderContext>.CurrentOperation, propertyInfoExpression),
                        Expression.Assign(
                            resolvedObjectParameter,
                            dynamicBuildContext.GetResolveDependencyExpression<TBuilderContext>(selectedProperty.Property.PropertyType, selectedProperty.Resolver)),
                        Expression.Call(
                            Expression.Convert(
                                BuilderContextExpression<TBuilderContext>.Existing,
                                dynamicBuildContext.TypeToBuild),
                            GetValidatedPropertySetter(selectedProperty.Property),
                            resolvedObjectParameter)));
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
