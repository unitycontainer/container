using Unity.Builder;
using Unity.Extension;
using Unity.Processors;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
        where TContext : IBuilderContext
    {



        public static ResolveDelegate<TContext> DefaultFactory(ref TContext context)
        {
            //switch (context.Registration?.CreationPolicy)
            //{
            //    case CreationPolicy.Once:
            //        return ((Policies<TContext>)context.Policies).ActivatePipeline;

            //    case CreationPolicy.Always:
            //        return PipelineCompiled(ref context);

            //    case CreationPolicy.OnceInWhile:
            //        return PipelineResolved(ref context);
            //}
            return ((Policies<TContext>)context.Policies).ActivatePipeline;
            //return PipelineCompiled(ref context); 
        }

        public static PipelineFactory<TContext> DefaultCompileProcessorFactory(MemberProcessor<TContext>[] chain)
        {
            return DefaultFactory;
        }
    }
}
