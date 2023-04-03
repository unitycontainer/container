using System.Drawing;
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
        public virtual void ResolverBuild<TContext>(ref TContext context, ref InjectionInfoStruct<TMemberInfo> info)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            var parameters = Unsafe.As<TMemberInfo>(info.MemberInfo!).GetParameters();

            if (0 == parameters.Length) return;

            info.InjectedValue[DataType.Value] = DataType.Array == info.InjectedValue.Type
                ? ResolverBuild(ref context, parameters, (object?[])info.InjectedValue.Value!)
                : ResolverBuild(ref context, parameters);
        }

        protected virtual object?[] ResolverBuild<TContext>(ref TContext context, ParameterInfo[] parameters, object?[] data)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            var arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                var parameter = parameters[index];
                var injected = data[index];
                var info = new InjectionInfoStruct<ParameterInfo>(parameter, parameter.ParameterType);

                ProvideParameterInfo(ref info);

                if (injected is IInjectionInfoProvider provider)
                    provider.ProvideInfo(ref info);
                else
                    info.Data = injected;

                AnalyzeInfo(ref context, ref info);
            }

            return arguments;
        }

        protected virtual object?[] ResolverBuild<TContext>(ref TContext context, ParameterInfo[] parameters)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            var resolvers = new ResolverPipeline[parameters.Length];

            for (var index = 0; index < resolvers.Length; index++)
            {
                var parameter = parameters[index];
                if (parameter.ParameterType.IsByRef)
                {
                    context.Error($"Parameter {parameter} is ref or out");
                    return EmptyParametersArray;
                }

                var info = new InjectionInfoStruct<ParameterInfo>(parameter, parameter.ParameterType);

                ProvideParameterInfo(ref info);
                AnalyzeInfo(ref context, ref info);

                var type = info.ContractType;
                var name = info.ContractName;
                var data = info.InjectedValue.Value;


                resolvers[index] = info switch
                {
                    { 
                        InjectedValue.Type: DataType.Value 
                    } 
                    => throw new NotImplementedException(),

                    {
                        InjectedValue.Type: DataType.Pipeline,
                        DefaultValue.Type: DataType.None
                    }
                    => throw new NotImplementedException(),

                    _ => throw new NotImplementedException(),
                };

            }

            return resolvers;
        }
    }
}
