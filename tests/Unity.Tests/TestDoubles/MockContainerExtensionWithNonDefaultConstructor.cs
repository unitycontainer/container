using Unity.Extension;

namespace Unity.Tests.TestDoubles
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
