using System.Linq.Expressions;
using Unity.Builder;
using Unity.Extension;
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
                var context = new BuildPlanContext<FactoryBuilderStrategy>(ref parent);

                while (!context.IsFaulted && ++i < delegates.Length)
                    delegates[i](ref context);

                var pipeline = context.Target ?? throw new InvalidOperationException();

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

            var factory = ChainConverter<FactoryBuilderStrategy, BuildPlanContext<FactoryBuilderStrategy>>.ChainToFactory(delegates);

            return (ref BuilderContext parent) =>
            {
                var context = new BuildPlanContext<FactoryBuilderStrategy>(ref parent);

                factory(ref context);
                var pipeline = context.Target!;

                return (ref BuilderContext c) => 
                {
                    pipeline(ref c);
                    return c.Target;
                };
            };
        }

        #endregion

        private static IEnumerable<ResolverBuildPlan> GetDelegates(MemberProcessor[] chain)
            => chain.Where(p =>
            {
                var info = p.GetType().GetMethod(nameof(MemberProcessor.BuildUp))!;
                return typeof(MemberProcessor) != info.DeclaringType;
            })
            .Select(p => (ResolverBuildPlan)p.BuildResolver);
    }
}
