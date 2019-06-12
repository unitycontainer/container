using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Lifetime;

namespace Unity
{
    public partial class LifetimePipeline : Pipeline
    {
        #region Fields

        private static readonly MethodInfo _recoverMethod = typeof(SynchronizedLifetimeManager)
            .GetTypeInfo().GetDeclaredMethod(nameof(SynchronizedLifetimeManager.Recover));

        private static readonly ParameterExpression _value = Expression.Variable(typeof(object), "value");
        private static readonly ParameterExpression _lifetime = Expression.Variable(typeof(LifetimeManager), "lifetime");

        private static readonly MethodCallExpression _contextGetLifetimeManager = Expression.Call(
            PipelineContextExpression.Context, PipelineContextExpression.GetMethod,
            Expression.Constant(typeof(LifetimeManager), typeof(Type)));

        private static readonly ConditionalExpression _setPerResolveSingleton = 
            Expression.IfThen(
                Expression.NotEqual(Expression.Constant(null), PipelineContextExpression.DeclaringType),
                SetPerBuildSingletonExpr);

        private static readonly InvocationExpression _lifetimeGetValue = Expression.Invoke(
            Expression.MakeMemberAccess(_lifetime, typeof(LifetimeManager).GetTypeInfo().GetDeclaredProperty(nameof(LifetimeManager.Get))),
            PipelineContextExpression.LifetimeContainer);

        private static readonly Expression ReturnIfResolvedAlready = Expression.Block(new[] { _lifetime, _value }, new Expression[] {
            Expression.Assign(_lifetime, Expression.Convert(_contextGetLifetimeManager, typeof(LifetimeManager))),
            Expression.IfThen(
                Expression.NotEqual(Expression.Constant(null), _lifetime),
                Expression.Block(new Expression[] {
                    Expression.Assign(_value, _lifetimeGetValue),
                    Expression.IfThen(
                        Expression.NotEqual(Expression.Constant(LifetimeManager.NoValue), _value),
                        Expression.Return(ReturnTarget, _value))}))});

        #endregion


        #region PipelineBuilder

        public override IEnumerable<Expression> Express(ref PipelineBuilder builder)
        {
            var expressions = builder.Express();

            return builder.LifetimeManager switch
            {
                SynchronizedLifetimeManager manager => SynchronizedLifetimeExpression(manager, expressions),
                PerResolveLifetimeManager _ => PerResolveLifetimeExpression(expressions),
                _ => expressions
            };
        }

        #endregion


        #region Implementation

        protected virtual IEnumerable<Expression> SynchronizedLifetimeExpression(SynchronizedLifetimeManager manager, IEnumerable<Expression> expressions)
        {
            var tryBody = Expression.Block(expressions);
            var catchBody = Expression.Catch(typeof(Exception), Expression.Block(
                    Expression.Call(Expression.Constant(manager), _recoverMethod),
                    Expression.Rethrow(tryBody.Type)));

            yield return Expression.TryCatch(tryBody, catchBody);
        }

        protected virtual IEnumerable<Expression> PerResolveLifetimeExpression(IEnumerable<Expression> expressons)
        {
            // Check and return if already resolved
            yield return ReturnIfResolvedAlready; 

            // Execute Pipeline
            foreach(var expression in expressons) yield return expression;

            // Save resolved value in per resolve singleton
            yield return _setPerResolveSingleton;
        }

        #endregion
    }
}
