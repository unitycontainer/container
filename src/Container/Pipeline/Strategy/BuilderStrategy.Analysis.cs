namespace Unity.Extension
{
    public abstract partial class BuilderStrategy
    {
        public virtual object? Analyse<TContext>(ref TContext context)
            where TContext : IBuilderContext => context.Contract.Type;
    }
}
