
using Unity.Extension;

namespace Unity.Tests.v5.TestSupport
{
    public class MockContainerExtension : UnityContainerExtension, IMockConfiguration
    {
        private bool initializeWasCalled = false;

        public bool InitializeWasCalled
        {
            get { return this.initializeWasCalled; }
        }

        public new IExtensionContext Context
        {
            get { return base.Context; }
        }

        protected override void Initialize()
        {
            this.initializeWasCalled = true;
        }
    }

    public interface IMockConfiguration : IUnityContainerExtensionConfigurator
    {
    }
}
