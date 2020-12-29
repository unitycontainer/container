using Unity.Container;
using Unity.Extension;

namespace Unity
{
    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract partial class RegistrationManager
    {
        #region Fields

        private ResolveDelegate<PipelineContext>? _pipeline;

        #endregion


        #region Pipeline

        public virtual ResolveDelegate<PipelineContext>? GetPipeline(Scope scope) 
            => _pipeline;

        public virtual ResolveDelegate<PipelineContext> SetPipeline(ResolveDelegate<PipelineContext> pipeline, Scope scope) 
            => _pipeline = pipeline;

        #endregion
    }
}
