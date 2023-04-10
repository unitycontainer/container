using System.Collections;
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

            var index = 0;
            var resolvers = new BuilderStrategyPipeline[members.Length];
            var enumerator = SelectMembers(ref context, members);

            while (enumerator.MoveNext())
            {
                ref var current = ref enumerator.Current;

                EvaluateInfo(ref context, ref current);
                resolvers[index++] = SetMemberValueResolver(current.MemberInfo, BuildResolver(ref context, ref current));
            }

            if (0 == index || context.IsFaulted) return;
            if (resolvers.Length != index) Array.Resize(ref resolvers, index);

            var upstream = context.Target!;

            context.Target = (ref BuilderContext context) =>
            {
                upstream(ref context);

                for(var i = 0; !context.IsFaulted && i < resolvers.Length; i++) 
                { 
                    resolvers[i](ref context);
                }
            };
        }

        protected virtual BuilderStrategyPipeline SetMemberValueResolver(TMemberInfo info, ResolverPipeline pipeline) 
            => throw new NotImplementedException();

        protected static ResolverPipeline BuildResolver<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            return info switch
            {
                { InjectedValue.Type: DataType.Array } => InjectedArrayResolver(ref context, ref info),
                { InjectedValue.Type: DataType.Value } => InjectedValueResolver(ref info),
                { InjectedValue.Type: DataType.Pipeline } => InjectedPipelineResolver(ref info),
                { InjectedValue.Type: DataType.Unknown } => throw new NotImplementedException(),

                {
                    InjectedValue.Type: DataType.None,
                    DefaultValue.Type: DataType.None
                } => RequiredResolver(ref info),

                _ => OptionalResolver(ref info),
            };
        }

        protected static ResolverPipeline InjectedArrayResolver<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            Debug.Assert(info.InjectedValue.Value is not null);
            Debug.Assert(info.ContractType.IsArray);

            var data = (object?[])info.InjectedValue.Value!;
            var type = info.ContractType.GetElementType()!;
            var resolvers = new ResolverPipeline[data.Length];

            for (var i = 0; i < data.Length; i++)
            {
                var import = info.With(type!, data[i]);

                EvaluateInfo(ref context, ref import);
                resolvers[i] = BuildResolver(ref context, ref import);
            }


            return (ref BuilderContext context) =>
            { 
                IList buffer = Array.CreateInstance(type, data.Length);

                for (var i = 0; i < resolvers.Length; i++)
                {
                    buffer[i] = resolvers[i](ref context);
                }

                return buffer;
            };
        }

        protected static ResolverPipeline InjectedValueResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            object? value = info.InjectedValue.Value;
            TMember member = info.MemberInfo;
            Contract contract = info.Contract;

            return (ref BuilderContext context) => context.OverrideValue(member, ref contract, value);
        }

        protected static ResolverPipeline InjectedPipelineResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            ResolverPipeline pipeline = (ResolverPipeline?)info.InjectedValue.Value ?? throw new InvalidOperationException();
            TMember member = info.MemberInfo;
            Contract contract = info.Contract;

            return (ref BuilderContext context) => context.OverridePipeline(member, ref contract, pipeline);
        }

        protected static ResolverPipeline RequiredResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            TMember member = info.MemberInfo;
            Contract contract = info.Contract;

            return (ref BuilderContext context) => context.Resolve(member, ref contract);
        }

        protected static ResolverPipeline OptionalResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            TMember member = info.MemberInfo;
            Contract contract = info.Contract;
            var value = info.DefaultValue.Value;

            return info.DefaultValue.Type switch
            {
                DataType.None => throw new NotImplementedException(),
                DataType.Array => throw new NotImplementedException(),
                DataType.Unknown => throw new NotImplementedException(),

                DataType.Value => (ref BuilderContext context) => context.ResolveOptional(member, ref contract, value),

                DataType.Pipeline => (ref BuilderContext context) => context.ResolveOptional(member, ref contract,
                                                                                            (ResolverPipeline?)value),

                _ => (ref BuilderContext context) => context.ResolveOptional(member, ref contract, (ResolverPipeline)GetDefaultValue)
            };
        }

        private static object? GetDefaultValue(ref BuilderContext context)
            => (context.TargetType.IsValueType && Nullable.GetUnderlyingType(context.TargetType) == null)
                ? Activator.CreateInstance(context.TargetType) : null;
    }
}
