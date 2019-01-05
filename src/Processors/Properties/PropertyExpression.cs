using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;

namespace Unity.Processors
{
    public partial class PropertyProcessor
    {


        #region Overrides

        protected override Expression GetResolverExpression(PropertyInfo property, string name, object resolver)
        {
            return Expression.Convert(
                Expression.Call(BuilderContextExpression.Context, ResolveProperty,
                    Expression.Constant(property, typeof(PropertyInfo)),
                    Expression.Constant(name, typeof(string)),
                    Expression.Constant(PreProcessResolver(property, resolver), typeof(object))),
                property.PropertyType);
        }

        protected override MemberExpression CreateMemberExpression(PropertyInfo info)
            => Expression.Property(Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType), info);

        #endregion
    }
}
