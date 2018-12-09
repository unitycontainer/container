namespace Unity.Resolution
{
    public delegate object ResolveDelegate<TContext>(ref TContext context) where TContext : IResolveContext;
}
