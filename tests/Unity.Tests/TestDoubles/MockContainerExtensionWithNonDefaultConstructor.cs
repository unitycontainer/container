using Unity.Extension;

namespace Unity.Tests.v5.TestDoubles
{
    public class ContainerExtensionWithNonDefaultConstructor : UnityContainerExtension
    {
        public ContainerExtensionWithNonDefaultConstructor(IUnityContainer container)
        {
        }

        protected override void Initialize()
        {
        }
    }
}
