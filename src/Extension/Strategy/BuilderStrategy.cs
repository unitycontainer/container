using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Strategies
{
    /// <summary>
    /// Represents a strategy in the chain of responsibility.
    /// Strategies are required to support both BuildUp and TearDown.
    /// </summary>
    public abstract partial class BuilderStrategy
    {
        #region Fields

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        private bool? _preBuildUp;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        private bool? _postBuildUp;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        private MethodInfo? _preBuildUpMethod;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        private MethodInfo? _postBuildUpMethod;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        private MethodCallExpression? _preCallExpr;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        private MethodCallExpression? _postCallExpr;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        private IEnumerable<Expression>? _preBuildExpr;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        private IEnumerable<Expression>? _postBuildExpr;

        #endregion

        
        #region Analysis

        public virtual object? Analyze<TContext>(ref TContext context)
           where TContext : IBuilderContext => context.Contract.Type;

        #endregion


        #region Implementation

        private bool IsPreBuildUp 
            => _preBuildUp
            ??= ReferenceEquals(typeof(BuilderStrategy), (_preBuildUpMethod 
                ??= GetType().GetMethod(nameof(PreBuildUp)))!.DeclaringType);

        private bool IsPostBuildUp
            => _postBuildUp
            ??= ReferenceEquals(typeof(BuilderStrategy), (_postBuildUpMethod 
                ??= GetType().GetMethod(nameof(PostBuildUp)))!.DeclaringType);

        private IEnumerable<Expression> GetPreBuildExpr<TBuilder, TContext>(ref TBuilder builder)
            where TBuilder : IExpressPipeline<TContext> where TContext : IBuilderContext
            => new Expression[]
            {
                _preCallExpr ??= Expression.Call(
                    Expression.Constant(this), _preBuildUpMethod!.MakeGenericMethod(typeof(TContext)), builder.ContextExpression),
                builder.ReturnIfFaultedExpression
            };

        private IEnumerable<Expression> GetPostBuildExpr<TBuilder, TContext>(ref TBuilder builder)
            where TBuilder : IExpressPipeline<TContext> where TContext : IBuilderContext
            => new Expression[]
            {
                builder.ReturnIfFaultedExpression,
                _postCallExpr ??= Expression.Call(
                    Expression.Constant(this), _postBuildUpMethod!.MakeGenericMethod(typeof(TContext)), builder.ContextExpression)
            };

        #endregion
    }
}
