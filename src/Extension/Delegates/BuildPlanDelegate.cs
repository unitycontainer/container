using Unity.Resolution;

namespace Unity.Extension
{
    public delegate void BuildPlanDelegate<TTarget, TContext>(ref TContext context)
        where TContext : IBuildPlanContext<TTarget>;
}
