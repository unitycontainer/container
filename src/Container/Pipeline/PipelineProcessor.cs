using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class PipelineProcessor
    {
        #region Constants

        public static Pipeline DefaultPipeline = (ref ResolutionContext context) => { };

        #endregion


        #region Visitors

        public virtual void BuildUpVisitor(ref PipelineBuilder<object?> builder) { }

        public virtual void ResolutionVisitor(ref PipelineBuilder<Pipeline?> builder) { }

        #endregion
    }
}
