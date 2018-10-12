using System;
using System.Linq.Expressions;
using Unity.Build;

namespace Unity.Factory
{
    public interface IExpressionFactory<out TExpression> 
        where TExpression : Expression
    {
        TExpression GetExpression<TContext>(Type type)
            where TContext : IBuildContext;
    }
}
