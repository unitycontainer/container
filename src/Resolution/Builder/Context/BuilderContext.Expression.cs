using System.Linq.Expressions;
using System.Reflection;

namespace Unity.Builder
{
    public partial struct BuilderContext
    {
        #region Filds

        private static readonly ParameterInfo _contextParameter
            = typeof(ResolverPipeline).GetMethod(nameof(ResolverPipeline.Invoke))!
                                      .GetParameters()
                                      .First();

        #endregion


        #region Expressions

        public static readonly ParameterExpression ContextExpression
            = Expression.Parameter(_contextParameter.ParameterType, _contextParameter.Name);


        public static readonly MemberExpression ExistingExpression
            = Expression.MakeMemberAccess(ContextExpression,
                typeof(IBuilderContext).GetProperty(nameof(IBuilderContext.Existing))!);


        public static readonly MemberExpression IsFaultedExpression
            = Expression.MakeMemberAccess(ContextExpression,
                typeof(IBuilderContext).GetProperty(nameof(IBuilderContext.IsFaulted))!);

        #endregion
    }
}
