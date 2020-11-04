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
        protected object?[] BuildParameters(ref PipelineContext context, ParameterInfo[] parameters, object?[] data)
        {
            Debug.Assert(parameters.Length == data.Length);

            ResolverOverride? @override;
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
            {
                var parameter = parameters[index];
                if (!IsValid(parameter, ref context)) return arguments;
                var info = InjectionInfoFromData(parameter, data[index]);

                // Check for override
                if (null != (@override = context.GetOverride(in info.Import)))
                {
                    if (@override.Value is IReflectionProvider<ParameterInfo> provider)
                    {
                        var providerInfo = provider.GetInfo(parameter);
                        arguments[index] = BuildImport(ref context, in providerInfo.Import, in providerInfo.Data);
                    }
                    else
                        arguments[index] = BuildImport(ref context, in info.Import, parameter.AsImportData(@override.Value));
                }
                else
                    arguments[index] = BuildImport(ref context, in info.Import, in info.Data);
            }

            return arguments;
        }

        protected object?[] BuildParameters(ref PipelineContext context, ParameterInfo[] parameters)
        {
            ResolverOverride? @override;
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
            {
                var parameter = parameters[index];
                if (!IsValid(parameter, ref context)) return arguments;
                var info = InjectionInfoFromParameter(parameter);

                // Check for override
                if (null != (@override = context.GetOverride(in info.Import)))
                {
                    if (@override.Value is IReflectionProvider<ParameterInfo> provider)
                    {
                        var providerInfo = provider.GetInfo(parameter);
                        arguments[index] = BuildImport(ref context, in providerInfo.Import, in providerInfo.Data);
                    }
                    else
                        arguments[index] = BuildImport(ref context, in info.Import, info.Import.Element.AsImportData(@override.Value));
                }
                else
                    arguments[index] = BuildImport(ref context, in info.Import);
            }

            return arguments;
        }

        protected object? BuildImport(ref PipelineContext context, in ImportInfo<ParameterInfo> import, in ImportData data)
        {
            if (ImportType.Value == data.DataType) return data.Value;

            ErrorInfo error = default;
            var contract = import.Contract;
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            local.Target = data.DataType switch
            {
                ImportType.None => context.Container.Resolve(ref local),

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

        protected object? BuildImport(ref PipelineContext context, in ImportInfo<ParameterInfo> import)
        {
            ErrorInfo error = default;
            var contract = import.Contract;
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            local.Target = context.Container.Resolve(ref local);

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
