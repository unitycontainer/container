using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Processors;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class BuildExpressionChainConverter
    {
        #region Factories

        public static FactoryPipeline ChainToIteratedBuildExpressionFactory(MemberProcessor[] chain)
        {
            var delegates = GetDelegates(chain).ToArray();

            return (ref BuilderContext parent) =>
            {
                var i = -1;
                var context = new BuildPlanContext<IEnumerable<Expression>>(ref parent);

                while (!context.IsFaulted && ++i < delegates.Length)
                    delegates[i](ref context);

                var pipeline = context.Target ?? throw new InvalidOperationException();

                return (ref BuilderContext c) =>
                {
                    return pipeline;
                };
            };
        }

        public static FactoryPipeline ChainToCompiledBuildExpressionFactory(MemberProcessor[] chain)
        {
            var delegates = GetDelegates(chain);
            var factory = ChainConverter<IEnumerable<Expression>, BuildPlanContext<IEnumerable<Expression>>>.ChainToFactory(delegates);

            return (ref BuilderContext parent) =>
            {
                var context = new BuildPlanContext<IEnumerable<Expression>>(ref parent);

                factory(ref context);

                return null;
            };
        }

        #endregion


        private static IEnumerable<ExpressionBuildPlan> GetDelegates(MemberProcessor[] chain)
            => chain.Where(p =>
            {
                var info = p.GetType().GetMethod(nameof(MemberProcessor.BuildExpression))!;
                return typeof(MemberProcessor) != info.DeclaringType;
            })
            .Select(p => (ExpressionBuildPlan)p.BuildExpression);
    }
}
