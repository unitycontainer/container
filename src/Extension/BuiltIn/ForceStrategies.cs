using System;
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
            context.Policies.Set<PipelineFactory<BuilderContext>>(Pipelines<BuilderContext>.PipelineActivated);
            context.Policies.Set<Policies<BuilderContext>.BuildUpPipelineFactory>(Pipelines<BuilderContext>.IteratedBuildUpPipelineFactory);
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
            context.Policies.Set<PipelineFactory<BuilderContext>>(Pipelines<BuilderContext>.PipelineCompiled);
            context.Policies.Set<Policies<BuilderContext>.BuildUpPipelineFactory>(Pipelines<BuilderContext>.CompiledBuildUpPipelineFactory);
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
            context.Policies.Set<PipelineFactory<BuilderContext>>(Pipelines<BuilderContext>.PipelineResolved);
            context.Policies.Set<Policies<BuilderContext>.BuildUpPipelineFactory>(Pipelines<BuilderContext>.ResolvedBuildUpPipelineFactory);
        }
    }
}
