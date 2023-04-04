using System.Diagnostics;
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

            info.InjectedValue[DataType.Pipeline] = 
                DataType.Array == info.InjectedValue.Type && info.InjectedValue.Value is not null
                ? ResolverBuild(ref context, parameters, (object?[])info.InjectedValue.Value)
                : ResolverBuild(ref context, parameters);
        }

        protected object?[] ResolverBuild<TContext>(ref TContext context, ParameterInfo[] parameters, object?[] data)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            Debug.Assert(data.Length == parameters.Length);

            var resolvers = new object?[parameters.Length];

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

                resolvers[index] = ResolverBuild(ref context, ref info);
            }

            return resolvers;
        }

        protected object?[] ResolverBuild<TContext>(ref TContext context, ParameterInfo[] parameters)
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

                resolvers[index] = ResolverBuild(ref context, ref info);
            }

            return resolvers;
        }

        private ResolverPipeline ResolverBuild<TContext>(ref TContext context, ref InjectionInfoStruct<ParameterInfo> info) 
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {            
            AnalyzeInfo(ref context, ref info);

            return info switch
            {
                { InjectedValue.Type: DataType.Value    } => InjectedValueResolver(ref info),
                { InjectedValue.Type: DataType.Pipeline } => InjectedPipelineResolver(ref info),
                { InjectedValue.Type: DataType.Array    } => InjectedArrayResolver(ref context, ref info),
                { InjectedValue.Type: DataType.Unknown  } => throw new NotImplementedException(),

                { InjectedValue.Type: DataType.None,
                   DefaultValue.Type: DataType.None     } => RequiredResolver(ref info),

                { InjectedValue.Type: DataType.None,
                   DefaultValue.Type: DataType.Value    } => WithDefaultResolver(ref info),

                { InjectedValue.Type: DataType.None,
                   DefaultValue.Type: DataType.Pipeline } => WithDefaultMethodResolver(ref info),

                _ => OptionalResolver(ref info),
            };
        }
    }
}
