using System.Linq.Expressions;
using System.Reflection;
using Unity.Extension;

namespace Unity.Builder
{
    public partial struct BuilderContext
    {
        #region Filds

        private static readonly ParameterInfo _contextParameter
            = typeof(BuilderStrategyDelegate<BuilderContext>).GetMethod(nameof(BuilderStrategyDelegate<BuilderContext>.Invoke))!
                                                .GetParameters()[0];

        #endregion


        #region Expressions

        public static readonly ParameterExpression ContextExpression
            = Expression.Parameter(_contextParameter.ParameterType, _contextParameter.Name);


        public static readonly MemberExpression ExistingExpression
            = Expression.MakeMemberAccess(ContextExpression,
                typeof(BuilderContext).GetProperty(nameof(IBuilderContext.Existing))!);

        #endregion
    }
}
