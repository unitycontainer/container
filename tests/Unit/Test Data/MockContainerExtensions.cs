using Unity.Extension;


namespace Unity.Container.Tests
{
    public interface IMockConfiguration
    {
        ExtensionContext Context { get; }
    }

    public interface IOtherConfiguration : IMockConfiguration
    { }

    public class MockContainerExtension : UnityContainerExtension, IMockConfiguration
    {
        public bool InitializeWasCalled { get; private set; } = false;

        ExtensionContext IMockConfiguration.Context => Context;


        protected override void Initialize() => InitializeWasCalled = true;
    }

    public class DerivedContainerExtension : MockContainerExtension
    {}

    public class OtherContainerExtension : UnityContainerExtension, IOtherConfiguration
    {
        public bool InitializeWasCalled { get; private set; } = false;

        ExtensionContext IMockConfiguration.Context => Context;

        protected override void Initialize() => InitializeWasCalled = true;
    }
}
