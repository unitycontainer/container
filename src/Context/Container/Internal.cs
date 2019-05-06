using Unity.Pipeline;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerContext
        {
            #region Pipeline Caches

            internal PipelineBuilder[] TypePipelineCache     { get; private set; }
            internal PipelineBuilder[] FactoryPipelineCache  { get; private set; }
            internal PipelineBuilder[] InstancePipelineCache { get; private set; }

            #endregion

        }
    }
}
