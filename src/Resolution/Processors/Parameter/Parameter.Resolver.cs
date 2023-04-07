using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Injection;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        public virtual void BuildResolver<TContext>(ref TContext context, ref InjectionInfoStruct<TMemberInfo> info)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            var parameters = Unsafe.As<TMemberInfo>(info.MemberInfo!).GetParameters();

            if (0 == parameters.Length) return;

            info.InjectedValue[DataType.Pipeline] = 
                DataType.Array == info.InjectedValue.Type && info.InjectedValue.Value is not null
                ? BuildResolver(ref context, parameters, (object?[])info.InjectedValue.Value)
                : BuildResolver(ref context, parameters);
        }

        protected object BuildResolver<TContext>(ref TContext context, ParameterInfo[] parameters, object?[] data)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            Debug.Assert(data.Length == parameters.Length);

            var resolvers = new ResolverPipeline[parameters.Length];

            for (var index = 0; index < resolvers.Length; index++)
            {
                var parameter = parameters[index];
                var injected = data[index];
                var info = new InjectionInfoStruct<ParameterInfo>(parameter, parameter.ParameterType);

                ProvideParameterInfo(ref info);

                if (injected is IInjectionInfoProvider provider)
                    provider.ProvideInfo(ref info);
                else
                    info.Data = injected;

                resolvers[index] = BuildResolver(ref context, ref info);
            }

            return BuildResolver(resolvers);
        }

        protected object BuildResolver<TContext>(ref TContext context, ParameterInfo[] parameters)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            var resolvers = new ResolverPipeline[parameters.Length];

            for (var index = 0; index < resolvers.Length; index++)
            {
                var parameter = parameters[index];
                if (parameter.ParameterType.IsByRef)
                {
                    context.Error($"Parameter {parameter} is ref or out");
                    return UnityContainer.NoValue;
                }

                var info = new InjectionInfoStruct<ParameterInfo>(parameter, parameter.ParameterType);

                ProvideParameterInfo(ref info);

                resolvers[index] = BuildResolver(ref context, ref info);
            }

            return BuildResolver(resolvers);
        }

        protected ResolverPipeline BuildResolver(ResolverPipeline[] resolvers)
        {
            return (ref BuilderContext context) =>
            {
                var result = new object?[resolvers.Length];

                for (var index = 0; !context.IsFaulted && index < resolvers.Length; index++)
                    result[index] = resolvers[index](ref context);

                return result;
            };
        }

        protected ResolverPipeline BuildResolver<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            AnalyzeInfo(ref context, ref info);

            return info switch
            {
                { InjectedValue.Type: DataType.Array }    => InjectedArrayResolver(ref context, ref info),
                { InjectedValue.Type: DataType.Value }    => InjectedValueResolver(ref info),
                { InjectedValue.Type: DataType.Pipeline } => InjectedPipelineResolver(ref info),
                { InjectedValue.Type: DataType.Unknown }  => throw new NotImplementedException(),

                {
                    InjectedValue.Type: DataType.None,
                    DefaultValue.Type: DataType.None
                } => RequiredResolver(ref info),

                _ => OptionalResolver(ref info),
            };
        }

        private static ResolverPipeline InjectedArrayResolver<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            Debug.Assert(info.InjectedValue.Value is not null);
            Debug.Assert(info.ContractType.IsArray);

            //            var data = (object?[])info.InjectedValue.Value!;
            //            var type = info.ContractType.GetElementType()!;

            //            IList buffer;

            //            try
            //            {
            //                buffer = Array.CreateInstance(type, data.Length);

            //                for (var i = 0; i < data.Length; i++)
            //                {
            //                    var import = info.With(type!, data[i]);

            //                    //BuildUpData<TContext, TMember, InjectionInfoStruct<TMember>>(ref context, ref import);
            //                    //BuildUpInfo(ref context, ref import);

            //                    if (context.IsFaulted)
            //                    {
            //                        info.InjectedValue = default;
            ////                        return;
            //                    }

            //                    buffer[i] = import.InjectedValue.Value;
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                context.Error(ex.Message);
            //                info.InjectedValue = default;
            ////                return;
            //            }



            throw new NotImplementedException();
        }

        private static ResolverPipeline InjectedValueResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            object? value = info.InjectedValue.Value;
            TMember member = info.MemberInfo;
            Contract contract = info.Contract;

            return (ref BuilderContext context) => context.Resolve(member, ref contract, value);
        }

        private static ResolverPipeline InjectedPipelineResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            ResolverPipeline pipeline = (ResolverPipeline?)info.InjectedValue.Value ?? throw new InvalidOperationException();
            TMember member = info.MemberInfo;
            Contract contract = info.Contract;

            return (ref BuilderContext context) => context.Resolve(member, ref contract, pipeline);
        }

        private static ResolverPipeline RequiredResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            TMember member = info.MemberInfo;
            Contract contract = info.Contract;

            return (ref BuilderContext context) => context.Resolve(member, ref contract);
        }

        private static ResolverPipeline OptionalResolver<TMember>(ref InjectionInfoStruct<TMember> info)
        {
            TMember member = info.MemberInfo;
            Contract contract = info.Contract;
            var value = info.DefaultValue.Value;

            return info.DefaultValue.Type switch
            {
                DataType.None => throw new NotImplementedException(),
                DataType.Array => throw new NotImplementedException(),
                DataType.Unknown => throw new NotImplementedException(),

                DataType.Value => (ref BuilderContext context) => context.Resolve(member, ref contract, value),

                DataType.Pipeline => (ref BuilderContext context) => context.Resolve(member, ref contract,
                                                                                            (ResolverPipeline?)value),

                _ => (ref BuilderContext context) => context.Resolve(member, ref contract, GetDefaultValue)
            };
        }

        private static object? GetDefaultValue(ref BuilderContext context)
            => (context.TargetType.IsValueType && Nullable.GetUnderlyingType(context.TargetType) == null)
                ? Activator.CreateInstance(context.TargetType) : null;
    }
}
