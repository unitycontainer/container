using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.Resolution;

namespace Unity
{
    public abstract class Pipeline
    {
        #region Public Members

        public virtual ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder builder) => builder.Pipeline();

        public virtual IEnumerable<Expression> Express(ref PipelineBuilder builder) => Enumerable.Empty<Expression>();

        #endregion
    }
}
