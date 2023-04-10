using System.Diagnostics;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Processors
{
    public partial class MethodProcessor
    {
        public override void BuildResolver<TContext>(ref TContext context)
        {
            Debug.Assert(null != context.Target, "Target should never be null");

            var members = GetDeclaredMembers(context.TargetType);

            if (0 == members.Length) return;

            var index = 0;
            var resolvers = new BuilderStrategyPipeline[members.Length];
            var enumerator = SelectMembers(ref context, members);

            while (enumerator.MoveNext())
            {
                ref var current = ref enumerator.Current;

                base.BuildResolver(ref context, ref current);
                var value = current.InjectedValue.Value!;

                resolvers[index++] = current.InjectedValue.Type switch
                {
                    DataType.None => SetMemberValueResolver(current.MemberInfo, EmptyArrayResolver),
                    DataType.Value => SetMemberValueResolver(current.MemberInfo, (ref BuilderContext context) => value),
                    DataType.Pipeline => SetMemberValueResolver(current.MemberInfo, (ResolverPipeline)value),
                    _ => throw new NotImplementedException(),
                };
            }

            if (0 == index || context.IsFaulted) return;
            if (resolvers.Length != index) Array.Resize(ref resolvers, index);

            var upstream = context.Target!;

            context.Target = (ref BuilderContext context) =>
            {
                upstream(ref context);

                for (var i = 0; !context.IsFaulted && i < resolvers.Length; i++)
                {
                    resolvers[i](ref context);
                }
            };
        }
    }
}
