using Unity.Extension;
using Unity.Extension;

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
        }
    }

    /// <summary>
    /// This extension forces the container to only use compiled strategies during resolution
    /// </summary>
    public class ForceCompillation : UnityContainerExtension
    {
        protected override void Initialize()
        {
        }
    }



    public class ForceBuildUp : UnityContainerExtension
    {
        protected override void Initialize()
        {
        }
    }
}
