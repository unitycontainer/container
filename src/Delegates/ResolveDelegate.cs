using Unity.Build;

namespace Unity.Delegates
{
    public delegate object ResolveDelegate<TContext>(ref TContext context) where TContext : IBuildContext;
}
