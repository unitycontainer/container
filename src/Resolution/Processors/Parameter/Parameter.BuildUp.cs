using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        protected override void BuildUpInfo<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
        {
            var parameters = Unsafe.As<TMemberInfo>(info.MemberInfo!).GetParameters();
            
            if (0 == parameters.Length) 
            {
                info.DataValue[DataType.Value] = ParameterProcessor<TMemberInfo>.EmptyParametersArray;
            }
            else 
            { 
                info.DataValue[DataType.Value] = DataType.Array == info.DataValue.Type
                    ? BuildUpParameters(ref context, parameters, (object?[])info.DataValue.Value!)
                    : BuildUpParameters(ref context, parameters);
            }
        }

        protected virtual object?[] BuildUpParameters<TContext>(ref TContext context, ParameterInfo[] parameters, object?[] data)
            where TContext : IBuilderContext
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

                var @override = context.GetOverride<ParameterInfo, InjectionInfoStruct<ParameterInfo>>(ref info);
                if (@override is not null) info.Data = @override.Resolve(ref context);

                AnalyzeInfo(ref context, ref info);
                base.BuildUpInfo(ref context, ref info);

                arguments[index] = !info.DataValue.IsValue && info.AllowDefault
                    ? GetDefaultValue(info.MemberType)
                    : info.DataValue.Value;
            }

            return arguments;
        }

        protected virtual object?[] BuildUpParameters<TContext>(ref TContext context, ParameterInfo[] parameters)
            where TContext : IBuilderContext
        {
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                var parameter = parameters[index];
                if (parameter.ParameterType.IsByRef) throw new ArgumentException($"Parameter {parameter} is ref or out");

                // Initialize member
                var info = new InjectionInfoStruct<ParameterInfo>(parameter, parameter.ParameterType);

                ProvideParameterInfo(ref info);

                var @override = context.GetOverride<ParameterInfo, InjectionInfoStruct<ParameterInfo>>(ref info);
                if (@override is not null) info.Data = @override.Resolve(ref context);

                AnalyzeInfo(ref context, ref info);
                base.BuildUpInfo(ref context, ref info);

                if (context.IsFaulted) return arguments;

                // TODO: requires optimization
                arguments[index] = !info.DataValue.IsValue && info.AllowDefault
                    ? GetDefaultValue(info.MemberType)
                    : info.DataValue.Value;
            }

            return arguments;
        }
    }
}
