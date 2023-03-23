using Unity.Strategies;

namespace Unity.Container
{
    public partial class MappingStrategy : BuilderStrategy
    {
        public override void PreBuildUp<TContext>(ref TContext context) 
            => context.Existing = context.MapTo(new Contract(context.Type, context.Contract.Name));
    }
}
