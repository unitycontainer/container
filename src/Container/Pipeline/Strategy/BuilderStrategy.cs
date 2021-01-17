using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity.Extension
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

        public virtual object? Analyse<TContext>(ref TContext context)
           where TContext : IBuilderContext => context.Contract.Type;

        #endregion


        #region Properties

        private bool IsPreBuildUp 
            => _preBuildUp
            ??= ReferenceEquals(typeof(BuilderStrategy), (_preBuildUpMethod 
                ??= GetType().GetMethod(nameof(PreBuildUp)))!.DeclaringType);

        private bool IsPostBuildUp
            => _postBuildUp
            ??= ReferenceEquals(typeof(BuilderStrategy), (_postBuildUpMethod 
                ??= GetType().GetMethod(nameof(PostBuildUp)))!.DeclaringType);

        #endregion


        #region Implementation

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


        #region v4

        /// <summary>
        /// Obsolete PreTearDown. No longer supported
        /// </summary>
        [Obsolete("PreTearDown method is deprecated. Tear down no longer supported", true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void PreTearDown(IBuilderContext context)
        {
        }

        /// <summary>
        /// Obsolete PostTearDown. No longer supported
        /// </summary>
        [Obsolete("PostTearDown method is deprecated. Tear down no longer supported", true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void PostTearDown(IBuilderContext context)
        {
        }

        #endregion


        #region v5

        /// <summary>
        /// Obsolete RequiredToBuildType. No longer supported
        /// </summary>
        [Obsolete("RequiredToBuildType method is deprecated", true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool RequiredToBuildType(IUnityContainer container, Type type)
        {
            return true;
        }


        /// <summary>
        /// Obsolete RequiredToResolveInstance. No longer supported
        /// </summary>
        [Obsolete("RequiredToResolveInstance method is deprecated", true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool RequiredToResolveInstance(IUnityContainer container)
        {
            return false;
        }

        #endregion
    }
}
