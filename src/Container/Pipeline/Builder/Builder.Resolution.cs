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
        #region IBuildPipeline

        public ResolveDelegate<TContext>? Build(ref TContext context)
        {
            return _enumerator.MoveNext()
                 ? _enumerator.Current.BuildPipeline(ref this, ref context)
                 : null;
        }

        #endregion
    }
}
