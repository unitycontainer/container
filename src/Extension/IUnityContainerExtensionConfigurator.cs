
namespace Unity.Extension
{
    /// <summary>
    /// Base interface for all extension configurations.
    /// </summary>
    public interface IUnityContainerExtensionConfigurator
    {
        /// <summary>
        /// The container that owns configured extension.
        /// </summary>
        UnityContainer? Container { get; }
    }
}
