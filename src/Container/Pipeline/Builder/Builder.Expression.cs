using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineBuilder<TContext> : IExpressPipeline<TContext>
    {

        public ResolveDelegate<TContext> CompilePipeline(object?[] analytics)
        {
            _analytics = analytics;

            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(
               Expression.Block(Express().Concat(PostfixExpression)),
               PipelineBuilder<TContext>._contextExpression);

            return lambda.Compile();
        }


        #region IExpressPipeline

        public ParameterExpression ContextExpression => _contextExpression;

        public ConditionalExpression ReturnIfFaultedExpression => _returnIfFaulted;

        public IEnumerable<Expression> Express()
        {
            if (_strategies.Length <= _index) return Enumerable.Empty<Expression>();

            var analytics = _analytics?[_index];
            var strategy = _strategies[_index++];

            return strategy.ExpressPipeline<PipelineBuilder<TContext>, TContext>(ref this, analytics);
        }


        #endregion
    }
}
