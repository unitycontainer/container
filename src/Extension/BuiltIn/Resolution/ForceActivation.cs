using Unity.Extension;

namespace Unity
{
    /// <summary>
    /// This extension forces the container to only use activated strategies
    /// </summary>
    public class ForceActivation : UnityContainerExtension
    {
        protected override void Initialize()
            => Initialize(Context!);

        public static void Initialize(ExtensionContext context)
        {

        }
    }
}
