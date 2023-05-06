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

                EvaluateInfo(ref context, ref info);
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
                EvaluateInfo(ref context, ref info);

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
    }
}
