using Unity.Builder;
using Unity.Extension;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
        where TContext : IBuilderContext
    {
        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            

            policies.Set<BuilderStrategy, Policies<TContext>.ChainToPipelineFactory>(CompiledBuildUpPipelineFactory);

            policies.Set<Policies<TContext>.ChainToPipelineFactory>(CompiledChainToPipelineFactory);            
            policies.Set<Policies<TContext>.CompileTypePipelineFactory>(DefaultCompileProcessorFactory);
        }


        public static ResolveDelegate<TContext> DefaultFactory(ref TContext context)
        {
            switch (context.Registration?.CreationPolicy)
            {
                case CreationPolicy.Once:
                    return ((Policies<TContext>)context.Policies).ActivatePipeline;

                case CreationPolicy.Always:
                    return PipelineCompiled(ref context);

                case CreationPolicy.OnceInWhile:
                    return PipelineResolved(ref context);
            }

            return PipelineCompiled(ref context); 
        }

        public static PipelineFactory<TContext> DefaultCompileProcessorFactory(IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain)
        {
            return DefaultFactory;
        }
    }
}
