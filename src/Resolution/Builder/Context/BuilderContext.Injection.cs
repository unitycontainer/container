namespace Unity.Builder
{
    public partial struct BuilderContext
    {
        public object? FromPipeline<TMember>(TMember member, ref Contract contract, ResolverPipeline pipeline)
        {
            var context = new BuilderContext(ref contract, ref this);

            var @override = GetOverride(member, ref contract);
            if (@override is not null) return @override.Resolve(ref context);

            return pipeline(ref context);
        }

        public object? FromInjectedValue<TMember>(TMember member, ref Contract contract, object? value)
        {
            var @override = GetOverride(member, ref contract);
            if (@override is not null)
            {
                var context = new BuilderContext(ref contract, ref this);
                return @override.Resolve(ref context);
            }
            
            return value;
        }
    }
}
