using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    public partial struct PipelineBuilder<TContext>
    {
        #region Fields

        private static readonly ParameterInfo _contextParameter 
            = typeof(PipelineDelegate<TContext>).GetMethod(nameof(PipelineDelegate<TContext>.Invoke))!
                                                .GetParameters()[0];
        
        public static readonly ParameterExpression ContextExpression 
            = Expression.Parameter(_contextParameter.ParameterType, _contextParameter.Name);

        public static readonly MemberExpression IsFaultedExpression =
            Expression.MakeMemberAccess(ContextExpression,
                typeof(TContext).GetProperty(nameof(IBuilderContext.IsFaulted))!);

        public static readonly LabelTarget ExitLabel = Expression.Label();
        public static readonly LabelExpression Label = Expression.Label(ExitLabel);
        private static readonly ConditionalExpression IfThenExpression 
            = Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), IsFaultedExpression),
                    Expression.Return(ExitLabel));

        public static readonly MemberExpression TargetExpression =
            Expression.MakeMemberAccess(ContextExpression,
                typeof(TContext).GetProperty(nameof(PipelineContext.Target))!);

        #endregion
    }
}
