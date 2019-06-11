using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Resolution;

namespace Unity
{
    public abstract class Pipeline
    {
        #region Fields

        protected static readonly BinaryExpression NullTestExpression = Expression.Equal(Expression.Constant(null), PipelineContextExpression.Existing);

        #endregion


        #region Public Members

        public virtual ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder builder) => builder.Pipeline();

        public virtual IEnumerable<Expression> Express(ref PipelineBuilder builder) => builder.Express();

        #endregion
    }
}
