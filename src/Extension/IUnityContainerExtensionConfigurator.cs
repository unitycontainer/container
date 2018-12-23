


namespace Unity.Extension
{
    /// <summary>
    /// Base interface for all extension configuration interfaces.
    /// </summary>
    public interface IUnityContainerExtensionConfigurator
    {
        /// <summary>
        /// Retrieve the container instance that we are currently configuring.
        /// </summary>
        IUnityContainer Container { get; }
    }
}
