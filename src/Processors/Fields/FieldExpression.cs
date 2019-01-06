using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;

namespace Unity.Processors
{
    public partial class FieldsProcessor 
    {


        #region Overrides

        protected override Expression GetResolverExpression(FieldInfo field, string name, object resolver)
        {
            return Expression.Convert(
                Expression.Call(BuilderContextExpression.Context, ResolveField,
                    Expression.Constant(field, typeof(FieldInfo)),
                    Expression.Constant(name, typeof(string)),
                    Expression.Constant(PreProcessResolver(field, resolver), typeof(object))),
                field.FieldType);
        }

        protected override Expression GetResolverExpression(FieldInfo field, string name, Expression expression)
        {
            var variable = Expression.Variable(typeof(object));
            var resolve  = Expression.Call(BuilderContextExpression.Context, ResolveField,
                Expression.Constant(field, typeof(FieldInfo)),
                Expression.Constant(name, typeof(string)),
                variable);

            return Expression.Block(new[] { variable }, new Expression[]
            {
                Expression.Assign(variable, expression),
                Expression.Convert(resolve, field.FieldType)
            });
        }

        protected override MemberExpression CreateMemberExpression(FieldInfo info)
            => Expression.Field(Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType), info);

        #endregion
    }
}
