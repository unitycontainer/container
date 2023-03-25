using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo>
    {
        protected override void BuildUp<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> descriptor)
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


        protected object?[] BuildUp<TContext>(ref TContext context, ParameterInfo[] parameters, object?[] data)
            where TContext : IBuilderContext
        {
            var arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                var import = new MemberDescriptor<TContext, ParameterInfo>(parameters[index]);
                var injected = data[index];

                ParameterProvider.ProvideInfo(ref import);

                if (injected is IInjectionProvider provider)
                    provider.ProvideInfo(ref import);
                else
                    import.Data = injected;

                var @override = context.GetOverride<ParameterInfo, MemberDescriptor<TContext, ParameterInfo>>(ref import);
                if (@override is not null) import.Data = @override.Resolve(ref context);

                base.BuildUp(ref context, ref import);

                if (context.IsFaulted) return arguments;

                arguments[index] = !import.ValueData.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : import.ValueData.Value;
            }

            return arguments;
        }

        protected object?[] BuildUp<TContext>(ref TContext context, ParameterInfo[] parameters)
            where TContext : IBuilderContext
        {
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                var parameter = parameters[index];
                if (parameter.ParameterType.IsByRef) throw new ArgumentException($"Parameter {parameter} is ref or out");

                // Initialize member
                var import = new MemberDescriptor<TContext, ParameterInfo>(parameter);

                ParameterProvider.ProvideInfo(ref import);

                var @override = context.GetOverride<ParameterInfo, MemberDescriptor<TContext, ParameterInfo>>(ref import);
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
