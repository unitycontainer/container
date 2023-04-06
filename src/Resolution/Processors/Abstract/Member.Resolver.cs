using System.Diagnostics;
using Unity.Builder;
using Unity.Injection;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TMemberInfo, TData>
    {
        public override void BuildResolver<TContext>(ref TContext context)
        {
            Debug.Assert(null != context.Target, "Target should never be null");
            var members = GetDeclaredMembers(context.TargetType);

            if (0 == members.Length) return;

            ResolverPipeline[] resolvers = new ResolverPipeline[0];
            // = ResolversFromSelection(type, members).Distinct().ToArray();
            var upstream = context.Target;

            context.Target = (ref BuilderContext context) =>
            {
                upstream(ref context);

                if (context.IsFaulted) return;

                foreach (var resolver in resolvers) resolver(ref context);
            };
        }

        //public virtual ResolveDelegate<BuilderContext> GetResolver(Type type, IPolicySet registration, ResolveDelegate<BuilderContext> seed);

    }
}
