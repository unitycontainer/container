using System.Collections.Generic;
using System.Linq.Expressions;

namespace Unity.Extension
{
    public interface IExpressPipeline<TContext>
        where TContext : IBuilderContext
    {
        #region Expressions

        ParameterExpression ContextExpression { get; }

        ConditionalExpression ReturnIfFaultedExpression { get; }

        #endregion


        IEnumerable<Expression> Express();
    }
}
