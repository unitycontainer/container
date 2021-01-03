using System.Collections.Generic;
using System.Linq;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
    {
        #region Factory

        public static ResolveDelegate<TContext> PipelineFromStagedChainFactory(IStagedStrategyChain strategies)
        {
            var processors = ((IEnumerable<BuilderStrategy>)strategies).ToArray();

            return (ref TContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i].PreBuildUp(ref context);

                while (!context.IsFaulted && --i >= 0)
                    processors[i].PostBuildUp(ref context);

                return context.Target;
            };
        }


        public static ResolveDelegate<TContext> CompiledChainFactory(IStagedStrategyChain strategies)
            =>  new PipelineBuilder<TContext>((IEnumerable<BuilderStrategy>)strategies).ExpressBuildUp();


        #endregion


        #region Implementation
        
        private static object? DummyPipeline(ref TContext _)
            => UnityContainer.NoValue;

        #endregion


        // TODO: Merge with factories
        //public static void Setup(ExtensionContext context)
        //{
        //    var policies = context.Policies;


        //    // TODO: Requires optimization
        //    ActivatePipeline = context.Policies.Get<ResolveDelegate<PipelineContext>>(
        //        typeof(Activator), OnActivatePipelineChanged)!;

        //    FactoryPipeline = context.Policies.Get<ResolveDelegate<PipelineContext>>(
        //        typeof(IUnityContainer.FactoryDelegate), OnFactoryPipelineChanged)!;

        //    InstancePipeline = context.Policies.Get<ResolveDelegate<PipelineContext>>(
        //        typeof(Policies<PipelineContext>.CategoryInstance), OnInstancePipelineChanged)!;
        //}




        #region Chains Changes

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static void OnActivatePipelineChanged(Type? target, Type type, object? policy)
        //    => ActivatePipeline = (ResolveDelegate<PipelineContext>)(policy ??
        //        throw new ArgumentNullException(nameof(policy)));

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static void OnFactoryPipelineChanged(Type? target, Type type, object? policy)
        //    => FactoryPipeline = (ResolveDelegate<PipelineContext>)(policy ??
        //        throw new ArgumentNullException(nameof(policy)));

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static void OnInstancePipelineChanged(Type? target, Type type, object? policy)
        //    => InstancePipeline = (ResolveDelegate<PipelineContext>)(policy ??
        //        throw new ArgumentNullException(nameof(policy)));

        #endregion
    }
}
