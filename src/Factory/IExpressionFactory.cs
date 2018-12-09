using System;
using System.Linq.Expressions;
using Unity.Resolution;

namespace Unity.Factory
{
    public interface IExpressionFactory<out TExpression> 
        where TExpression : Expression
    {
        TExpression GetExpression<TContext>(Type type)
            where TContext : IResolveContext;
    }
}
