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
            .GetTypeInfo().GetDeclaredMethod(nameof(SynchronizedLifetimeManager.Recover));

        private static readonly ConstructorInfo _perResolveInfo = typeof(RuntimePerResolveLifetimeManager)
            .GetTypeInfo().DeclaredConstructors.First();

        protected static readonly Expression SetPerBuildSingletonExpr =
            Expression.Call(PipelineContextExpression.Context,
                PipelineContextExpression.SetMethod,
                Expression.Constant(typeof(LifetimeManager), typeof(Type)),
                Expression.New(_perResolveInfo, PipelineContextExpression.Existing));

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

        protected virtual IEnumerable<Expression> PerResolveLifetimeExpression(IEnumerable<Expression> expressions)
        {
            Expression exp;
            try
            {
                ParameterExpression _valueExpr = Expression.Variable(typeof(object));
                ParameterExpression _lifetimeExpr = Expression.Variable(typeof(LifetimeManager));

                MethodCallExpression _getManagerExpr = Expression.Call(
                    PipelineContextExpression.Context,
                    PipelineContextExpression.GetMethod,
                    Expression.Constant(typeof(LifetimeManager), typeof(Type)));


                exp = Expression.IfThenElse(NullTestExpression,
                    Expression.Block(new[] { _lifetimeExpr, _valueExpr }, new Expression[]
                    {
                        Expression.Assign(_lifetimeExpr, Expression.Convert(_getManagerExpr, typeof(LifetimeManager))),
                        //Expression.IfThen(
                        //    Expression.NotEqual(Expression.Constant(null), _lifetimeExpr),
                        //    Expression.Assign()),
                        Expression.Block(expressions)
                    }),
                    Expression.Block(expressions));

            }
            catch (Exception ex)
            {
                throw;
            }

            yield return exp;
        }

        #endregion
    }
}
