using Unity.Resolution;
using Unity.Strategies;

namespace Unity.Container
{
    internal static partial class Pipelines<TContext>
    {
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
                    ? (ref TContext context) =>
                    {
                        strategy.PreBuildUp(ref context);
                        if (context.IsFaulted) return context.Existing;

                        strategy.PostBuildUp(ref context);
                        return context.Existing;
                    }
                    : (ref TContext context) =>
                    {
                        strategy.PreBuildUp(ref context);
                        if (context.IsFaulted) return context.Existing;

                        // Run downstream pipeline
                        pipeline(ref context);
                        if (context.IsFaulted) return context.Existing;

                        strategy.PostBuildUp(ref context);
                        return context.Existing;
                    };
            }
            else if (pre)
            {
                return pipeline is null
                    ? (ref TContext context) =>
                    {
                        strategy.PreBuildUp(ref context);
                        return context.Existing;
                    }
                    : (ref TContext context) =>
                    {
                        strategy.PreBuildUp(ref context);

                        // Run downstream pipeline
                        return context.IsFaulted
                        ? context.Existing
                        : pipeline(ref context);
                    };
            }
            else if (post)
            {
                return pipeline is null
                    ? (ref TContext context) =>
                    {
                        strategy.PostBuildUp(ref context);
                        return context.Existing;
                    }
                    : (ref TContext context) =>
                    {
                        // Run downstream pipeline
                    
                        pipeline(ref context);
                        if (context.IsFaulted) return context.Existing;

                        strategy.PostBuildUp(ref context);
                        return context.Existing;
                    };
            }

            return pipeline;
        }
    }
}
