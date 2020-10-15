using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        protected object?[] Build(ref PipelineContext context, ParameterInfo[] parameters, object?[] data)
        {
            Debug.Assert(parameters.Length == data.Length);

            ResolverOverride? @override;
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
            {
                var parameter = parameters[index];
                var info = parameter.AsInjectionInfo(data[index]);

                // Check for override
                if (null != (@override = context.GetOverride(in info.Import)))
                {
                    if (@override.Value is IReflectionProvider<ParameterInfo> provider)
                    {
                        var providerInfo = provider.GetInfo(parameter);
                        arguments[index] = Build(ref context, in providerInfo.Import, in providerInfo.Data);
                    }
                    else
                        arguments[index] = Build(ref context, in info.Import, parameter.AsImportData(@override.Value));
                }
                else
                    arguments[index] = Build(ref context, in info.Import, in info.Data);
            }

            return arguments;
        }

        protected object?[] Build(ref PipelineContext context, ParameterInfo[] parameters)
        {
            ResolverOverride? @override;
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
            {
                var parameter = parameters[index];
                var info = parameter.AsInjectionInfo();

                // Check for override
                if (null != (@override = context.GetOverride(in info.Import)))
                {
                    if (@override.Value is IReflectionProvider<ParameterInfo> provider)
                    {
                        var providerInfo = provider.GetInfo(parameter);
                        arguments[index] = Build(ref context, in providerInfo.Import, in providerInfo.Data);
                    }
                    else
                        arguments[index] = Build(ref context, in info.Import, info.Import.Element.AsImportData(@override.Value));
                }
                else
                    arguments[index] = Build(ref context, in info.Import);
            }

            return arguments;
        }

        protected object? Build(ref PipelineContext context, in ImportInfo<ParameterInfo> import, in ImportData data)
        {
            if (ImportType.Value == data.DataType) return data.Value;

            ErrorInfo error = default;
            var contract = import.Contract;
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            local.Target = data.DataType switch
            {
                ImportType.None => local.Resolve(),

                ImportType.Pipeline => local.GetValueRecursively(import.Element,
                    ((ResolveDelegate<PipelineContext>)data.Value!).Invoke(ref local)),

                    // TODO: Requires proper handling
                _ => local.Error("Invalid Import Type"),
            };

            if (local.IsFaulted && import.AllowDefault)
            {
                local.Target = import.Element.HasDefaultValue
                    ? import.Element.DefaultValue
                    : GetDefaultValue(import.Element.ParameterType);
            }

            return local.Target;
        }

        protected object? Build(ref PipelineContext context, in ImportInfo<ParameterInfo> import)
        {
            ErrorInfo error = default;
            var contract = import.Contract;
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            local.Target = local.Resolve();

            if (local.IsFaulted && import.AllowDefault)
            {
                local.Target = import.Element.HasDefaultValue
                    ? import.Element.DefaultValue
                    : GetDefaultValue(import.Element.ParameterType);
            }

            return local.Target;
        }


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? GetDefaultValue(Type t)
            => (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                ? Activator.CreateInstance(t) : null;

        #endregion
    }
}
