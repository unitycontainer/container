namespace Unity.Build
{
    public delegate object BuildDelegate<TContext>(ref TContext context) where TContext : IBuildContext;
}
