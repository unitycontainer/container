using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Container;

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
        MethodCallExpression? PreBuildCall;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        MethodCallExpression? PostBuildCall;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        ConditionalExpression? IfErrorExpression;

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

        internal IEnumerable<Expression> ExpressBuildUp<TBuilder, TContext>(ref TBuilder builder)
            where TBuilder : IExpressPipeline<TContext>
            where TContext : IBuilderContext
        {
            var pre = IsPreBuildUp;
            var post = IsPostBuildUp;

            ///////////////////////////////////////////////////////////////////
            // No overridden methods
            if (pre && post) return builder.BuildUp();

            ///////////////////////////////////////////////////////////////////
            // PreBuildUP is overridden
            if (!pre & post)
                return PreBuildExpression<TContext>(builder.BuildUp());

            ///////////////////////////////////////////////////////////////////
            // PostBuildUP is overridden
            if (pre & !post)
                return PostBuildExpression<TContext>(builder.BuildUp());

            ///////////////////////////////////////////////////////////////////
            // Both methods are overridden
            return BuildUpExpression<TContext>(builder.BuildUp());
        }

        private IEnumerable<Expression> PreBuildExpression<TContext>(IEnumerable<Expression> pipeline)
            where TContext : IBuilderContext
        {
            yield return PreBuildCall
                ??= Expression.Call(Expression.Constant(this),
                    _preBuildUpMethod!.MakeGenericMethod(typeof(TContext)),
                    PipelineBuilder<TContext>.ContextExpression);

            yield return IfErrorExpression
                ??= Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), PipelineBuilder<TContext>.IsFaultedExpression),
                    Expression.Return(PipelineBuilder<TContext>.ExitLabel));

            foreach (var expression in pipeline) yield return expression;
        }

        private IEnumerable<Expression> PostBuildExpression<TContext>(IEnumerable<Expression> pipeline)
            where TContext : IBuilderContext
        {
            foreach (var expression in pipeline) yield return expression;

            yield return IfErrorExpression
                ??= Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), PipelineBuilder<TContext>.IsFaultedExpression),
                    Expression.Return(PipelineBuilder<TContext>.ExitLabel));

            yield return PostBuildCall
                ??= Expression.Call(Expression.Constant(this),
                    _postBuildUpMethod!.MakeGenericMethod(typeof(TContext)),
                    PipelineBuilder<TContext>.ContextExpression);
        }

        private IEnumerable<Expression> BuildUpExpression<TContext>(IEnumerable<Expression> pipeline)
            where TContext : IBuilderContext
        {
            yield return PreBuildCall
                ??= Expression.Call(Expression.Constant(this),
                    _preBuildUpMethod!.MakeGenericMethod(typeof(TContext)),
                    PipelineBuilder<TContext>.ContextExpression);

            yield return IfErrorExpression
                ??= Expression.IfThen(
                    Expression.Equal(Expression.Constant(true), PipelineBuilder<TContext>.IsFaultedExpression),
                    Expression.Return(PipelineBuilder<TContext>.ExitLabel));

            foreach (var expression in pipeline) yield return expression;

            yield return IfErrorExpression;

            yield return PostBuildCall
                ??= Expression.Call(Expression.Constant(this),
                    _postBuildUpMethod!.MakeGenericMethod(typeof(TContext)),
                    PipelineBuilder<TContext>.ContextExpression);
        }

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
