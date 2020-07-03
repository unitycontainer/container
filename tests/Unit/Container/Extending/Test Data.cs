#if NET45
using Microsoft.Practices.Unity;
#else
using Unity.Extension;
#endif

namespace Container.Extending
{
    public interface IMockConfiguration
    {
        ExtensionContext ExtensionContext { get; }
    }

    public interface IOtherConfiguration : IMockConfiguration
    { }

    public class MockContainerExtension : UnityContainerExtension, 
                                          IMockConfiguration, 
                                          IUnityContainerExtensionConfigurator
    {
        public bool InitializeWasCalled { get; private set; } = false;

        public ExtensionContext ExtensionContext => Context;


        protected override void Initialize() => InitializeWasCalled = true;
    }

    public class DerivedContainerExtension : MockContainerExtension, 
                                             IOtherConfiguration
    { }

    public class OtherContainerExtension : UnityContainerExtension, 
                                           IOtherConfiguration
    {
        public bool InitializeWasCalled { get; private set; } = false;

        public ExtensionContext ExtensionContext => Context;

        protected override void Initialize() => InitializeWasCalled = true;
    }

    public class UnrelatedExtension : UnityContainerExtension,
                                      IUnityContainerExtensionConfigurator
    {
        public bool InitializeWasCalled { get; private set; } = false;

        public ExtensionContext ExtensionContext => Context;

        protected override void Initialize() => InitializeWasCalled = true;
    }

    public class SubscriberExtension : UnityContainerExtension
    {
        public bool InitializeWasCalled { get; private set; } = false;

        public ExtensionContext ExtensionContext => Context;
        public ExtensionContext ChildContext { get; private set; }

        protected override void Initialize()
        {
            InitializeWasCalled = true;
            Context.ChildContainerCreated += OnChildContainerCreated;
        }

        private void OnChildContainerCreated(ExtensionContext child) => ChildContext = child;
    }

}
