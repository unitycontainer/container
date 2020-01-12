using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Lifetime;

namespace Unity
{
    public partial class LifetimePipeline : Pipeline
    {
        #region Fields

        private static readonly MethodInfo _recoverMethod = typeof(SynchronizedLifetimeManager)
            .GetTypeInfo().GetDeclaredMethod(nameof(SynchronizedLifetimeManager.Recover))!;

        private static readonly Expression[] _setPerResolveSingleton = new Expression[] {
            Expression.IfThen(
                Expression.NotEqual(Expression.Constant(null), PipelineContext.DeclaringTypeExpression),
                SetPerBuildSingletonExpr) };

        private static readonly ParameterExpression _variable = Expression.Variable(typeof(UnityContainer.ContainerContext));
        private static readonly ParameterExpression[] _variables = new[] { _variable };
        private static readonly Expression _saveContainerContext = Expression.Assign(_variable, PipelineContext.ContainerContextExpression);
        private static readonly Expression _restoreContainerContext = Expression.Assign(PipelineContext.ContainerContextExpression, _variable);

        #endregion


        #region PipelineBuilder

        public override IEnumerable<Expression> Express(ref PipelineBuilder builder)
        {
            var expressions = builder.Express();

            return builder.LifetimeManager switch
            {
                PerResolveLifetimeManager _ 
                when null == builder.LifetimeManager?.Scope   => expressions.Concat(_setPerResolveSingleton),
                PerResolveLifetimeManager _ 
                when null != builder.LifetimeManager?.Scope   => PerResolveLifetimeExpression(expressions, builder.LifetimeManager.Scope),
                SynchronizedLifetimeManager manager           => SynchronizedLifetimeExpression(expressions, manager),
                _ when null != builder.LifetimeManager?.Scope => OtherLifetimeExpression(expressions, builder.LifetimeManager.Scope),
                _                                             => expressions
            };
        }

        #endregion


        #region Implementation

        protected virtual IEnumerable<Expression> SynchronizedLifetimeExpression(IEnumerable<Expression> expressions, SynchronizedLifetimeManager manager)
        {
            if (null == manager.Scope)
            {
                var tryBody = Expression.Block(expressions);
                var catchBody = Expression.Catch(typeof(Exception), Expression.Block(
                        Expression.Call(Expression.Constant(manager), _recoverMethod),
                        Expression.Rethrow(tryBody.Type)));

                yield return Expression.TryCatch(tryBody, catchBody);
            }
            else
            {
                var tryBody = Expression.Block(expressions);
                var catchBody = Expression.Catch(typeof(Exception), Expression.Block(
                        Expression.Call(Expression.Constant(manager), _recoverMethod),
                        Expression.Rethrow(tryBody.Type)));

                yield return Expression.Block(_variables,
                    _saveContainerContext,
                    Expression.Assign(PipelineContext.ContainerContextExpression, 
                                      Expression.Constant(manager.Scope, typeof(UnityContainer.ContainerContext))),
                    Expression.TryCatchFinally(tryBody, _restoreContainerContext, catchBody));
            }
        }

        protected virtual IEnumerable<Expression> PerResolveLifetimeExpression(IEnumerable<Expression> expressions, object scope)
        {
            yield return Expression.Block(_variables,
                _saveContainerContext,
                Expression.Assign(PipelineContext.ContainerContextExpression, 
                                  Expression.Constant(scope, typeof(UnityContainer.ContainerContext))),
                Expression.TryFinally(Expression.Block(expressions.Concat(_setPerResolveSingleton)), _restoreContainerContext));
        }

        protected virtual IEnumerable<Expression> OtherLifetimeExpression(IEnumerable<Expression> expressions, object scope)
        {
            yield return Expression.Block(_variables,
                _saveContainerContext,
                Expression.Assign(PipelineContext.ContainerContextExpression, 
                                  Expression.Constant(scope, typeof(UnityContainer.ContainerContext))),
                Expression.TryFinally(Expression.Block(expressions), _restoreContainerContext));
        }

        #endregion
    }
}
