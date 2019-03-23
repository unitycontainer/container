using Unity.Extension;
using Unity.Policy;

namespace Unity
{
    /// <summary>
    /// This extension forces the container to only use activated strategies during resolution
    /// </summary>
    /// <remarks>
    /// This extension forces compatibility with systems without support for runtime compilers. 
    /// One of such systems is iOS.
    /// </remarks>
    public class ForceActivation : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var unity = (UnityContainer)Container;

            unity._buildStrategy = unity.ResolvingFactory;
            unity.Defaults.Set(typeof(ResolveDelegateFactory), unity._buildStrategy);
        }
    }

    /// <summary>
    /// This extension forces the container to only use compiled strategies during resolution
    /// </summary>
    public class ForceCompillation : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var unity = (UnityContainer)Container;

            unity._buildStrategy = unity.CompilingFactory;
            unity.Defaults.Set(typeof(ResolveDelegateFactory), unity._buildStrategy);

        }
    }

}
