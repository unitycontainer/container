using Unity.Extension;

namespace Unity
{
    /// <summary>
    /// This extension forces the container to only use resolved strategies
    /// </summary>
    public class ForceResolution : UnityContainerExtension
    {
        protected override void Initialize()
            => Initialize(Context!);

        public static void Initialize(ExtensionContext context)
        {

        }
    }
}
