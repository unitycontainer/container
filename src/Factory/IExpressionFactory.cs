using System;
using System.Linq.Expressions;
using Unity.Build;
using Unity.Delegates;

namespace Unity.Factory
{
    public interface IExpressionFactory
    {
        Expression<ResolveDelegate<TContext>> GetExpression<TContext>(Type type)
            where TContext : IBuildContext;
    }
}
