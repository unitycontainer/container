using Unity.Extension;

namespace Unity.Tests.v5.Extension
{
    public interface IMyCustomConfigurator : IUnityContainerExtensionConfigurator
    {
        IMyCustomConfigurator AddPolicy();
    }
}