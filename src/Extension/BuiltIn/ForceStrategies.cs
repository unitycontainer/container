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
            => context.Policies
                      .Set<PipelineFactory<BuilderContext>>(typeof(Type), Pipelines.PipelineActivated);
    }


    /// <summary>
    /// This extension forces the container to only use compiled strategies
    /// </summary>
    public class ForceCompillation : UnityContainerExtension
    {
        protected override void Initialize()
            => Initialize(Context!);

        public static void Initialize(ExtensionContext context)
            => context.Policies
                      .Set<PipelineFactory<BuilderContext>>(typeof(Type), Pipelines.PipelineCompiled);
    }


    /// <summary>
    /// This extension forces the container to only use resolved strategies
    /// </summary>
    public class ForceResolution : UnityContainerExtension
    {
        protected override void Initialize()
            => Initialize(Context!);

        public static void Initialize(ExtensionContext context)
            => context.Policies
                      .Set<PipelineFactory<BuilderContext>>(typeof(Type), Pipelines.PipelineResolved);
    }
}
