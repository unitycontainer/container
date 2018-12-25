using System;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Injection;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;
using Unity.Strategies;


namespace Unity.Builder.Strategies
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that generates expressions to resolve fields
    /// on an object being built.
    /// </summary>
    public class DynamicMethodFieldsSetterStrategy : BuilderStrategy
    {
        #region BuilderStrategy

        /// <summary>
        /// Called during the chain of responsibility for a build operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override void PreBuildUp(ref BuilderContext context)
        {
            var dynamicBuildContext = (DynamicBuildPlanGenerationContext)context.Existing;

            var selector = GetPolicy<ISelect<FieldInfo>>(ref context,
                context.RegistrationType, context.RegistrationName);

            foreach (var field in selector.Select(ref context))
            {
                ParameterExpression resolvedObjectParameter;

                switch (field)
                {
                    case FieldInfo fieldInfo:
                        resolvedObjectParameter = Expression.Parameter(fieldInfo.FieldType);

                        //dynamicBuildContext.AddToBuildPlan(
                        //    Expression.Block(
                        //        new[] { resolvedObjectParameter },
                        //        Expression.Assign(
                        //            resolvedObjectParameter,
                        //            BuilderContextExpression.Resolve(fieldInfo,
                        //                                             context.RegistrationName,
                        //                                             fieldInfo.GetResolver())),
                        //        Expression.Call(
                        //            Expression.Convert(
                        //                BuilderContextExpression.Existing,
                        //                dynamicBuildContext.TypeToBuild),
                        //            GetValidatedFieldSetter(fieldInfo),
                        //            resolvedObjectParameter)));
                        break;

                    case MemberInfoMember<FieldInfo> injectionField:
                        var (info, value) = injectionField.FromType(context.Type);
                        resolvedObjectParameter = Expression.Parameter(info.FieldType);

                        //dynamicBuildContext.AddToBuildPlan(
                        //    Expression.Block(
                        //        new[] { resolvedObjectParameter },
                        //        Expression.Assign(
                        //            resolvedObjectParameter,
                        //            BuilderContextExpression.Resolve(info,
                        //                                             context.RegistrationName,
                        //                                             value)),
                        //        Expression.Call(
                        //            Expression.Convert(
                        //                BuilderContextExpression.Existing,
                        //                dynamicBuildContext.TypeToBuild),
                        //            GetValidatedFieldSetter(info),
                        //            resolvedObjectParameter)));
                        break;

                    default:
                        throw new InvalidOperationException("Unknown type of field");
                }
            }
        }

        #endregion


        #region Implementation

        //private static MethodInfo GetValidatedFieldSetter(FieldInfo field)
        //{
        //    throw new NotImplementedException();
        //    //// TODO: Check - Added a check for private to meet original expectations;
        //    //var setter = field..GetSetMethod(true);
        //    //if (setter == null || setter.IsPrivate)
        //    //{
        //    //    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
        //    //        Constants.FieldNotSettable, field.Name, field.DeclaringType?.FullName));
        //    //}
        //    //return setter;
        //}

        #endregion
    }
}
