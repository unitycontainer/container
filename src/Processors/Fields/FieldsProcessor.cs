using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Expressions;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public class FieldsProcessor : MemberBuildProcessor<FieldInfo, object>
    {
        #region Overrides

        public override IEnumerable<object> Select(ref BuilderContext context) =>
            base.Select(ref context).Distinct();

        #endregion

        #region BuilderStrategy

        /// <inheritdoc />
        public override IEnumerable<Expression> GetEnumerator(ref BuilderContext context)
        {
            var selector = GetPolicy<ISelect<FieldInfo>>(ref context, context.RegistrationType, context.RegistrationName);
            var fields = selector.Select(ref context);

            return GetEnumerator(context.Type, context.Name, context.Variable, fields);
        }

        #endregion


        #region Implementation

        private IEnumerable<Expression> GetEnumerator(Type type, string name, ParameterExpression variable, IEnumerable<object> fields)
        {
            foreach (var field in fields)
            {
                MemberExpression fieldExpr;

                switch (field)
                {
                    case FieldInfo fieldInfo:
                        fieldExpr = Expression.Field(variable, fieldInfo);
                        yield return Expression.Assign(fieldExpr, BuilderContextExpression.Resolve(fieldInfo, name));
                        break;

                    case MemberInfoMember<FieldInfo> injectionField:
                        var (info, value) = injectionField.FromType(type);
                        fieldExpr = Expression.Field(variable, info);
                        yield return Expression.Assign(fieldExpr, BuilderContextExpression.Resolve(info, name, value));
                        break;

                    default:
                        throw new InvalidOperationException("Unknown type of field");
                }
            }
        }


        public static Expression GetField(FieldInfo info, object value)
        {
            // Resolve all DependencyAttributes on this parameter, if any
            var attribute = info.GetCustomAttributes(false)
                .OfType<DependencyResolutionAttribute>()
                .FirstOrDefault();

            if (null == attribute)
                return BuilderContextExpression.Resolve(info, null, null);
            else
                return attribute is OptionalDependencyAttribute
                    ? BuilderContextExpression.Resolve(info, attribute.Name, Expression.Constant(null, typeof(object)))
                    : BuilderContextExpression.Resolve(info, attribute.Name, null);
        }

        #endregion

    }
}
