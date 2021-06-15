using Unity.Extension;

namespace Unity.Tests.v5.TestDoubles
{
    internal class MockContainerExtension : UnityContainerExtension, IMockConfiguration
    {
        private bool initializeWasCalled = false;

        public bool InitializeWasCalled
        {
            get { return this.initializeWasCalled; }
        }

        public new ExtensionContext Context
        {
            get { return base.Context; }
        }

        protected override void Initialize()
        {
            this.initializeWasCalled = true;
        }
    }

    internal interface IMockConfiguration : IUnityContainerExtensionConfigurator
    {   
    }
}
