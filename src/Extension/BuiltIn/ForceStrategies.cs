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
            context.Policies.Set<ChainToPipelineConverter>(Pipelines<BuilderContext>.ChainToStrategiesIteratedFactory);
            context.Policies.Set<ChainToFactoryConverter>(Pipelines<BuilderContext>.ChainToBuildUpIteratedFactory);
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
            context.Policies.Set<ChainToPipelineConverter>(Pipelines<BuilderContext>.ChainToStrategiesCompiledFactory);
            context.Policies.Set<ChainToFactoryConverter>(Pipelines<BuilderContext>.ChainToExpressionCompiledFactory);
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
            context.Policies.Set<ChainToPipelineConverter>(Pipelines<BuilderContext>.ChainToStrategiesResolvedFactory);
            context.Policies.Set<ChainToFactoryConverter>(Pipelines<BuilderContext>.ChainToResolverResolvedFactory);
        }
    }
}
