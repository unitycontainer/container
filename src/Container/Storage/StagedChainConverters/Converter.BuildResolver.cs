using Unity.Builder;
using Unity.Processors;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class BuildResolverChainConverter
    {

        #region Factories

        public static FactoryPipeline ChainToIteratedResolverFactory(MemberProcessor[] chain)
        {
            var delegates = GetDelegates(chain).ToArray();

            return (ref BuilderContext parent) =>
            {
                var i = -1;
                var context = new BuildPlanContext<BuilderStrategyPipeline>(ref parent);

                while (!context.IsFaulted && ++i < delegates.Length)
                    delegates[i](ref context);

                var pipeline = context.Target
                            ?? ((ref BuilderContext c) => c.Existing = UnityContainer.NoValue);

                return (ref BuilderContext c) =>
                {
                    pipeline(ref c);
                    return c.Target;
                };
            };
        }

        public static FactoryPipeline ChainToCompiledBuildResolverFactory(MemberProcessor[] chain)
        {
            var delegates = GetDelegates(chain);
            var factory = ChainConverter<BuilderStrategyPipeline>.ChainToFactory(delegates);

            return (ref BuilderContext parent) =>
            {
                var context = new BuildPlanContext<BuilderStrategyPipeline>(ref parent);

                factory(ref context);

                var pipeline = context.Target 
                            ?? ((ref BuilderContext c) => c.Existing = UnityContainer.NoValue);

                return (ref BuilderContext c) => 
                {
                    pipeline(ref c);
                    return c.Existing;
                };
            };
        }

        #endregion

        private static IEnumerable<BuildPlanResolverPipeline> GetDelegates(MemberProcessor[] chain)
            => chain.Where(p =>
            {
                var info = p.GetType().GetMethod(nameof(MemberProcessor.BuildUp))!;
                return typeof(MemberProcessor) != info.DeclaringType;
            })
            .Select(p => (BuildPlanResolverPipeline)p.BuildResolver);
    }
}
