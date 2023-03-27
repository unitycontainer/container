using Unity.Builder;
using Unity.Container;
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
            // TODO: context.Policies.Set<PipelineFactory<BuilderContext>>(Pipelines<BuilderContext>.PipelineActivated);
            context.Policies.Set<ChainToPipelineConverter>(Pipelines<BuilderContext>.IteratedChainToPipelineFactory);
            // TODO: Rebuild pipelines
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
            // TODO: context.Policies.Set<PipelineFactory<BuilderContext>>(Pipelines<BuilderContext>.PipelineCompiled);
            context.Policies.Set<ChainToPipelineConverter>(Pipelines<BuilderContext>.CompiledChainToPipelineFactory);
            // TODO: Rebuild pipelines
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
            // TODO: context.Policies.Set<PipelineFactory<BuilderContext>>(Pipelines<BuilderContext>.PipelineResolved);
            context.Policies.Set<ChainToPipelineConverter>(Pipelines<BuilderContext>.ResolvedChainToPipelineFactory);
            // TODO: Rebuild pipelines
        }
    }
}
