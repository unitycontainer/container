using Unity;
using Unity.Resolution;

namespace Unity
{
    public abstract class Pipeline
    {
        #region Public Members

        public virtual ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder builder) => builder.Pipeline();

        #endregion
    }
}
