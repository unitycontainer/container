using System.Reflection;
using System.Runtime.CompilerServices;
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

            info.DataValue[DataType.Value] = DataType.Array == info.DataValue.Type
                ? ResolverBuild(ref context, parameters, (object?[])info.DataValue.Value!)
                : ResolverBuild(ref context, parameters);
        }

        protected virtual object?[] ResolverBuild<TContext>(ref TContext context, ParameterInfo[] parameters, object?[] data)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            var arguments = new object?[parameters.Length];

            //for (var index = 0; index < arguments.Length; index++)
            //{
            //    var parameter = parameters[index];
            //    var injected = data[index];
            //    var info = new InjectionInfoStruct<ParameterInfo>(parameter, parameter.ParameterType);

            //    ProvideParameterInfo(ref info);

            //    if (injected is IInjectionInfoProvider provider)
            //        provider.ProvideInfo(ref info);
            //    else
            //        info.Data = injected;

            //    var @override = context.GetOverride<ParameterInfo, InjectionInfoStruct<ParameterInfo>>(ref info);
            //    if (@override is not null) info.Data = @override.Resolve(ref context);

            //    AnalyzeInfo(ref context, ref info);
            //    base.BuildUpInfo(ref context, ref info);

            //    arguments[index] = !info.DataValue.IsValue && info.AllowDefault
            //        ? GetDefaultValue(info.MemberType)
            //        : info.DataValue.Value;
            //}

            return arguments;
        }

        protected virtual object?[] ResolverBuild<TContext>(ref TContext context, ParameterInfo[] parameters)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        {
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                var parameter = parameters[index];
                if (parameter.ParameterType.IsByRef)
                {
                    context.Error($"Parameter {parameter} is ref or out");
                    return EmptyParametersArray;
                }

                var info = new InjectionInfoStruct<ParameterInfo>(parameter, parameter.ParameterType);

                ProvideParameterInfo(ref info);

                ////AnalyzeInfo(ref context, ref info);

                //if (context.IsFaulted) return EmptyParametersArray;

                //// TODO: requires optimization
                //arguments[index] = !info.DataValue.IsValue && info.AllowDefault
                //    ? GetDefaultValue(info.MemberType)
                //    : info.DataValue.Value;
            }

            return arguments;
        }
    }
}
