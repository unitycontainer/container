using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo>
    {
        protected override ImportData BuildUp<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> descriptor)
        {
            var parameters = Unsafe.As<TMemberInfo>(descriptor.MemberInfo!).GetParameters();
            if (0 == parameters.Length) return new ImportData(EmptyParametersArray, ImportType.Value);

            var arguments = ImportType.Arguments == descriptor.ValueData.Type
                ? BuildUp(ref context, parameters, (IList)descriptor.ValueData.Value!)
                : BuildUp(ref context, parameters);

            return new ImportData(arguments, ImportType.Value);
        }


        protected object?[] BuildUp<TContext>(ref TContext context, ParameterInfo[] parameters, IList data)
            where TContext : IBuilderContext
        {
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                var import = new MemberDescriptor<TContext, ParameterInfo>(parameters[index]);

                ParameterProvider.ProvideImport<TContext, MemberDescriptor<TContext, ParameterInfo>>(ref import);
                import.Dynamic = data[index];

                var @override = context.GetOverride<ParameterInfo, MemberDescriptor<TContext, ParameterInfo>>(ref import);
                if (@override is not null) import.Dynamic = @override.Value;

                var finalData = base.BuildUp(ref context, ref import);

                if (context.IsFaulted) return arguments;

                arguments[index] = !finalData.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : finalData.Value;
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

                ParameterProvider.ProvideImport<TContext, MemberDescriptor<TContext, ParameterInfo>>(ref import);

                var @override = context.GetOverride<ParameterInfo, MemberDescriptor<TContext, ParameterInfo>>(ref import);
                if (@override is not null) import.Dynamic = @override.Value;

                var finalData = base.BuildUp(ref context, ref import);

                if (context.IsFaulted) return arguments;

                // TODO: requires optimization
                arguments[index] = !finalData.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : finalData.Value;
            }

            return arguments;
        }
    }
}
