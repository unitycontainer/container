namespace Unity.Resolution;



public delegate void BuildPlanStrategyDelegate<TTarget, TContext>(ref TContext context)
    where TContext : IBuildPlanContext<TTarget>;
