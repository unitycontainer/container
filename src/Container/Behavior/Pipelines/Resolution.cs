using Unity.Resolution;
using Unity.Strategies;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
    {
        #region Implementation

        private static ResolveDelegate<TContext>? ResolvedBuildUp(BuilderStrategy[] chain, int level = 0)
        {
            if (chain.Length <= level) return null;

            var pipeline = ResolvedBuildUp(chain, level + 1);

            var strategy = chain[level];
            var type = strategy.GetType();

            var preBuildUpMethod = type.GetMethod(nameof(BuilderStrategy.PreBuildUp))!;
            var pre = !ReferenceEquals(typeof(BuilderStrategy), preBuildUpMethod.DeclaringType);

            var postBuildUpMethod = type.GetMethod(nameof(BuilderStrategy.PostBuildUp))!;
            var post = !ReferenceEquals(typeof(BuilderStrategy), postBuildUpMethod.DeclaringType);

            // Both methods are overridden
            if (pre && post)
            {
                return pipeline is null
                    ? PreBuildUpAndPostBuildUp(strategy)
                    : PreBuildUpAndPostBuildUp(strategy, pipeline);
            }
            else if (pre)
            {
                return pipeline is null
                    ? PreBuildUp(strategy)
                    : PreBuildUp(strategy, pipeline);
            }
            else if (post)
            {
                return pipeline is null
                    ? PostBuildUp(strategy)
                    : PostBuildUp(strategy, pipeline);
            }

            return pipeline;
        }

        private static ResolveDelegate<TContext> PreBuildUp(BuilderStrategy strategy)
            => (ref TContext context) =>
            {
                strategy.PreBuildUp(ref context);
                return context.Existing;
            };

        private static ResolveDelegate<TContext> PreBuildUp(BuilderStrategy strategy, ResolveDelegate<TContext> pipeline)
            => (ref TContext context) =>
            {
                strategy.PreBuildUp(ref context);

                // Run downstream pipeline
                return context.IsFaulted
                ? context.Existing
                : pipeline(ref context);
            };

        private static ResolveDelegate<TContext> PostBuildUp(BuilderStrategy strategy)
            => (ref TContext context) =>
            {
                strategy.PostBuildUp(ref context);
                return context.Existing;
            };

        private static ResolveDelegate<TContext> PostBuildUp(BuilderStrategy strategy, ResolveDelegate<TContext> pipeline)
            => (ref TContext context) =>
            {
                // Run downstream pipeline

                pipeline(ref context);
                if (context.IsFaulted) return context.Existing;

                strategy.PostBuildUp(ref context);
                return context.Existing;
            };

        private static ResolveDelegate<TContext> PreBuildUpAndPostBuildUp(BuilderStrategy strategy)
            => (ref TContext context) =>
            {
                strategy.PreBuildUp(ref context);
                if (context.IsFaulted) return context.Existing;

                strategy.PostBuildUp(ref context);
                return context.Existing;
            };

        private static ResolveDelegate<TContext> PreBuildUpAndPostBuildUp(BuilderStrategy strategy, ResolveDelegate<TContext> pipeline)
            => (ref TContext context) =>
            {
                strategy.PreBuildUp(ref context);
                if (context.IsFaulted) return context.Existing;

                // Run downstream pipeline
                pipeline(ref context);
                if (context.IsFaulted) return context.Existing;

                strategy.PostBuildUp(ref context);
                return context.Existing;
            };
        
        #endregion
    }
}
