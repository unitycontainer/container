using Unity.Extension;

namespace Unity
{
    /// <summary>
    /// This extension forces the container to only use compiled strategies
    /// </summary>
    public class ForceCompillation : UnityContainerExtension
    {
        protected override void Initialize()
            => Initialize(Context!);

        public static void Initialize(ExtensionContext context)
        { 
        
        }
    }
}
