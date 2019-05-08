

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerContext
        {
            #region Pipeline Caches

            internal Pipeline[] TypePipelineCache     { get; private set; }
            internal Pipeline[] FactoryPipelineCache  { get; private set; }
            internal Pipeline[] InstancePipelineCache { get; private set; }

            #endregion

        }
    }
}
