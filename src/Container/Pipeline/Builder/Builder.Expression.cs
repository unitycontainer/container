using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Expressions;
using Unity.Extension;

namespace Unity.Container
{
    public partial struct PipelineBuilder<TContext> : IBuildPipeline<TContext>,
                                                      IExpressPipeline<TContext>
    {
        #region IExpressPipeline


        public ResolveDelegate<TContext> CompilePipeline(object?[] analytics)
        {
            _analytics = analytics;

            var expressions = Express();
            var postfix = new Expression[] { PipelineBuilder<TContext>.Label, TargetExpression };
            
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(
               Expression.Block(expressions.Concat(postfix)),
               PipelineBuilder<TContext>.ContextExpression);

            return lambda.Compile();
        }

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
