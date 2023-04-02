using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Resolution
{
    public partial struct BuildPlanContext<TTarget>
    {
        #region Filds

        private static readonly ParameterInfo _contextParameter
            = typeof(BuildPlanStrategyDelegate<TTarget, BuildPlanContext<TTarget>>)
                .GetMethod(nameof(BuildPlanStrategyDelegate<TTarget, BuildPlanContext<TTarget>>.Invoke))!
                .GetParameters()
                .First();

        #endregion


        #region Expressions

        public static readonly ParameterExpression ContextExpression
            = Expression.Parameter(_contextParameter.ParameterType, _contextParameter.Name);


        public static readonly MemberExpression TargetExpression
            = Expression.MakeMemberAccess(ContextExpression,
                typeof(IBuildPlanContext<TTarget>).GetProperty(nameof(IBuildPlanContext<TTarget>.Target))!);


        public static readonly MemberExpression IsFaultedExpression
            = Expression.MakeMemberAccess(ContextExpression,
                typeof(IBuildPlanContext).GetProperty(nameof(IBuildPlanContext.IsFaulted))!);

        #endregion
    }
}
