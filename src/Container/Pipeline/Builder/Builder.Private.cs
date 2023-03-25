using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Extension;

namespace Unity.Container
{
    public partial struct PipelineBuilder<TContext>
    {
        #region Context

        private static readonly ParameterInfo _contextParameter 
            = typeof(PipelineDelegate<TContext>).GetMethod(nameof(PipelineDelegate<TContext>.Invoke))!
                                                .GetParameters()[0];
        
        private static readonly ParameterExpression _contextExpression 
            = Expression.Parameter(_contextParameter.ParameterType, _contextParameter.Name);

        private static readonly MemberExpression _isFaultedExpression =
            Expression.MakeMemberAccess(_contextExpression, typeof(TContext).GetProperty(nameof(IBuilderContext.IsFaulted))!);

        public static readonly MemberExpression TargetExpression =
            Expression.MakeMemberAccess(_contextExpression,
                typeof(TContext).GetProperty(nameof(BuilderContext.Existing))!);

        #endregion


        #region Operators

        public static readonly LabelTarget ExitLabel = Expression.Label();

        public static readonly LabelExpression Label = Expression.Label(ExitLabel);

        private static readonly ConditionalExpression _returnIfFaulted 
            = Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), _isFaultedExpression),
                    Expression.Return(ExitLabel));

        private static readonly IEnumerable<Expression> PostfixExpression = 
            new Expression[] { PipelineBuilder<TContext>.Label, TargetExpression };

        #endregion
    }
}
