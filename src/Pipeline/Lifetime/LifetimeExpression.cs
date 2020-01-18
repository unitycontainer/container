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

        #endregion


        #region PipelineBuilder

        public override IEnumerable<Expression> Express(ref PipelineBuilder builder)
        {
            var expressions = builder.Express();

            return builder.LifetimeManager switch
            {
                PerResolveLifetimeManager _    => expressions.Concat(_setPerResolveSingleton),
                SynchronizedLifetimeManager manager           => SynchronizedLifetimeExpression(expressions, manager),
                _                                             => expressions
            };
        }

        #endregion


        #region Implementation

        protected virtual IEnumerable<Expression> SynchronizedLifetimeExpression(IEnumerable<Expression> expressions, SynchronizedLifetimeManager manager)
        {
            var tryBody = Expression.Block(expressions);
            var catchBody = Expression.Catch(typeof(Exception), Expression.Block(
                    Expression.Call(Expression.Constant(manager), _recoverMethod),
                    Expression.Rethrow(tryBody.Type)));

            yield return Expression.TryCatch(tryBody, catchBody);
        }

        #endregion
    }
}
