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


        public ResolveDelegate<TContext> Compile()
        {
            var expressions = Express();
            var postfix = new Expression[] { PipelineBuilder<TContext>.Label, TargetExpression };
            // TODO: Optimization
            var lambda = Expression.Lambda<ResolveDelegate<TContext>>(
               Expression.Block(expressions.Concat(postfix)),
               PipelineBuilder<TContext>.ContextExpression);

            return lambda.Compile();
        }

        public IEnumerable<Expression> Express()
        {
            throw new NotImplementedException();
            //if (!_enumerator.MoveNext()) return EmptyExpression;

            //return _enumerator.Current.ExpressPipeline<PipelineBuilder<TContext>, TContext>(ref this);
        }


        #endregion
    }
}
