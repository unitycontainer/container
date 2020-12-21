namespace Unity.Extension
{
    public delegate object? ResolveDelegate<TContext>(ref TContext context)
        where TContext : IResolveContext;
}
