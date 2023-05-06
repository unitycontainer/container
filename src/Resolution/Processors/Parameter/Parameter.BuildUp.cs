using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        protected void BuildUpParameters<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
            where TContext : IBuilderContext
        {
            var parameters = Unsafe.As<TMemberInfo>(info.MemberInfo!).GetParameters();
            
            if (0 == parameters.Length) 
            {
                info.InjectedValue[DataType.Value] = ParameterProcessor<TMemberInfo>.EmptyParametersArray;
                return;
            }

            info.InjectedValue[DataType.Value] = DataType.Array == info.InjectedValue.Type
                ? BuildUpParameters(ref context, parameters, (object?[])info.InjectedValue.Value!)
                : BuildUpParameters(ref context, parameters);
        }

        protected virtual object?[] BuildUpParameters<TContext>(ref TContext context, ParameterInfo[] parameters, object?[] data)
            where TContext : IBuilderContext
        {
            var arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                var parameter = parameters[index];
                if (parameter.ParameterType.IsByRef) throw new ArgumentException($"Parameter {parameter} is ref or out");

                var injected = data[index];
                var info = new InjectionInfoStruct<ParameterInfo>(parameter, parameter.ParameterType);

                ProvideParameterInfo(ref info);

                if (injected is IInjectionInfoProvider provider)
                    provider.ProvideInfo(ref info);
                else
                    info.Data = injected;

                EvaluateInfo(ref context, ref info);
                BuildUpInfo(ref context, ref info);

                arguments[index] = !info.InjectedValue.IsValue && info.AllowDefault
                    ? GetDefaultValue(parameter.ParameterType)
                    : info.InjectedValue.Value;
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

                EvaluateInfo(ref context, ref info);
                BuildUpInfo(ref context, ref info);

                if (context.IsFaulted) return arguments;

                // TODO: requires optimization
                arguments[index] = !info.InjectedValue.IsValue && info.AllowDefault
                    ? GetDefaultValue(parameter.ParameterType)
                    : info.InjectedValue.Value;
            }

            return arguments;
        }
    }
}
