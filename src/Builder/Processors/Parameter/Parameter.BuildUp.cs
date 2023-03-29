using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class ParameterProcessor<TContext, TMemberInfo>
    {
        protected override void BuildUp<TMember>(ref TContext context, ref MemberInjectionInfo<TMember> descriptor)
        {
            var parameters = Unsafe.As<TMemberInfo>(descriptor.MemberInfo!).GetParameters();
            
            if (0 == parameters.Length) 
            {
                descriptor.ValueData[ImportType.Value] = EmptyParametersArray;
            }
            else 
            { 
                descriptor.ValueData[ImportType.Value] = ImportType.Array == descriptor.ValueData.Type
                    ? BuildUp(ref context, parameters, (object?[])descriptor.ValueData.Value!)
                    : BuildUp(ref context, parameters);
            }
        }


        protected object?[] BuildUp(ref TContext context, ParameterInfo[] parameters, object?[] data)
        {
            var arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                var import = new MemberInjectionInfo<ParameterInfo>(parameters[index]);
                var injected = data[index];

                ProvideParameterInfo(ref import);

                if (injected is IInjectionInfoProvider provider)
                    provider.ProvideInfo(ref import);
                else
                    import.Data = injected;

                var @override = context.GetOverride<ParameterInfo, MemberInjectionInfo<ParameterInfo>>(ref import);
                if (@override is not null) import.Data = @override.Resolve(ref context);

                base.BuildUp(ref context, ref import);

                if (context.IsFaulted) return arguments;

                arguments[index] = !import.ValueData.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : import.ValueData.Value;
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
                var import = new MemberInjectionInfo<ParameterInfo>(parameter);

                ProvideParameterInfo(ref import);

                var @override = context.GetOverride<ParameterInfo, MemberInjectionInfo<ParameterInfo>>(ref import);
                if (@override is not null) import.Data = @override.Resolve(ref context);

                base.BuildUp(ref context, ref import);

                if (context.IsFaulted) return arguments;

                // TODO: requires optimization
                arguments[index] = !import.ValueData.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : import.ValueData.Value;
            }

            return arguments;
        }
    }
}
