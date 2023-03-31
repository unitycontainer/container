using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class ParameterProcessor<TContext, TMemberInfo>
    {
        protected override void BuildUp<TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
        {
            var parameters = Unsafe.As<TMemberInfo>(info.MemberInfo!).GetParameters();
            
            if (0 == parameters.Length) 
            {
                info.DataValue[Storage.ValueType.Value] = ParameterProcessor<TContext, TMemberInfo>.EmptyParametersArray;
            }
            else 
            { 
                info.DataValue[Storage.ValueType.Value] = Storage.ValueType.Array == info.DataValue.Type
                    ? BuildUp(ref context, parameters, (object?[])info.DataValue.Value!)
                    : BuildUp(ref context, parameters);
            }
        }


        protected object?[] BuildUp(ref TContext context, ParameterInfo[] parameters, object?[] data)
        {
            var arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                var parameter = parameters[index];
                var import = new InjectionInfoStruct<ParameterInfo>(parameter, parameter.ParameterType);
                var injected = data[index];

                ProvideParameterInfo(ref import);

                if (injected is IInjectionInfoProvider provider)
                    provider.ProvideInfo(ref import);
                else
                    import.Data = injected;

                var @override = context.GetOverride<ParameterInfo, InjectionInfoStruct<ParameterInfo>>(ref import);
                if (@override is not null) import.Data = @override.Resolve(ref context);

                base.BuildUp(ref context, ref import);

                if (context.IsFaulted) return arguments;

                arguments[index] = !import.DataValue.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : import.DataValue.Value;
            }

            return arguments;
        }

        protected object?[] BuildUp(ref TContext context, ParameterInfo[] parameters)
        {
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                var parameter = parameters[index];
                if (parameter.ParameterType.IsByRef) throw new ArgumentException($"Parameter {parameter} is ref or out");

                // Initialize member
                var import = new InjectionInfoStruct<ParameterInfo>(parameter, parameter.ParameterType);

                ProvideParameterInfo(ref import);

                var @override = context.GetOverride<ParameterInfo, InjectionInfoStruct<ParameterInfo>>(ref import);
                if (@override is not null) import.Data = @override.Resolve(ref context);

                base.BuildUp(ref context, ref import);

                if (context.IsFaulted) return arguments;

                // TODO: requires optimization
                arguments[index] = !import.DataValue.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : import.DataValue.Value;
            }

            return arguments;
        }
    }
}
