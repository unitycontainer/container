using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Container;
using Unity.Lifetime;

namespace Unity.BuiltIn
{
    public partial class LifetimeProcessor
    {
        #region Fields

        //private static readonly MethodInfo _recoverMethod = typeof(SynchronizedLifetimeManager)
        //    .GetTypeInfo().GetDeclaredMethod(nameof(SynchronizedLifetimeManager.Recover))!;

        //private static readonly Expression[] _setPerResolveSingleton = new Expression[] {
        //    Expression.IfThen(
        //        Expression.NotEqual(Expression.Constant(null), PipelineContext.DeclaringTypeExpression),
        //        SetPerBuildSingletonExpr) };

        #endregion


        #region PipelineBuilder

        public override IEnumerable<Expression> Express(ref PipelineBuilder<IEnumerable<Expression>> builder)
        {
            throw new System.NotImplementedException();
            //var expressions = builder.Express();

            //return builder.LifetimeManager switch
            //{
            //    PerResolveLifetimeManager _    => expressions.Concat(_setPerResolveSingleton),
            //    SynchronizedLifetimeManager manager           => SynchronizedLifetimeExpression(expressions, manager),
            //    _                                             => expressions
            //};
        }

        #endregion


        #region Implementation

        protected virtual IEnumerable<Expression> SynchronizedLifetimeExpression(IEnumerable<Expression> expressions, SynchronizedLifetimeManager manager)
        {
            throw new System.NotImplementedException();
            //var tryBody = Expression.Block(expressions);
            //var catchBody = Expression.Catch(typeof(Exception), Expression.Block(
            //        Expression.Call(Expression.Constant(manager), _recoverMethod),
            //        Expression.Rethrow(tryBody.Type)));

            //yield return Expression.TryCatch(tryBody, catchBody);
        }

        #endregion
    }
}
