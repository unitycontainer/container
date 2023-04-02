using Unity.Builder;
using Unity.Container;
using Unity.Extension;
using Unity.Policy;

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
            context.Policies.Set<ChainToPipelineConverter>(BuildUpChainConverter.ChainToIteratedBuildUpPipeline);
            context.Policies.Set<ChainToFactoryConverter>(BuildUpChainConverter.ChainToBuildUpIteratedFactory);
        }
    }


    /// <summary>
    /// This extension forces the container to only use compiled strategies
    /// </summary>
    public class ForceCompilation : UnityContainerExtension
    {
        protected override void Initialize()
            => Initialize(Context!);

        public static void Initialize(ExtensionContext context)
        {
            context.Policies.Set<ChainToPipelineConverter>(BuildUpChainConverter.ChainToCompiledBuildUpPipeline);
            context.Policies.Set<ChainToFactoryConverter>(BuildExpressionChainConverter.ChainToCompiledBuildExpressionFactory);
        }
    }


    /// <summary>
    /// This extension forces the container to only use resolved strategies
    /// </summary>
    public class ForceResolution : UnityContainerExtension
    {
        protected override void Initialize()
            => Initialize(Context!);

        public static void Initialize(ExtensionContext context)
        {
            context.Policies.Set<ChainToPipelineConverter>(BuildUpChainConverter.ChainToResolvedBuildUpPipeline);
            context.Policies.Set<ChainToFactoryConverter>(BuildResolverChainConverter.ChainToIteratedResolverFactory);
            
        }
    }
}
