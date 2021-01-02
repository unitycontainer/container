using System.Collections.Generic;
using System.Linq.Expressions;

namespace Unity.Extension
{
    public interface IExpressPipeline<TContext>
        where TContext : IBuilderContext
    {
        IEnumerable<Expression> Express();

        IEnumerable<Expression> BuildUp();
    }
}
