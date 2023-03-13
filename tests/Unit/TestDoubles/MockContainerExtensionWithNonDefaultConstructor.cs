using Unity.Extension;

namespace Unit.Test.TestDoubles
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
