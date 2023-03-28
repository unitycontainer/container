using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Resolution;

namespace Unity.Container
{
    public partial class MappingStrategy<TContext> where TContext : IBuilderContext
    {
        public static ResolveDelegate<TContext> DefaultPipeline { get; } 
            = (ref TContext context) => context.Existing = context.MapTo(new Contract(context.Type, context.Contract.Name));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BuilderStrategyDelegate(ref TContext context)
            => context.Existing = DefaultPipeline;
    }
}
