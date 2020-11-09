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

            ReflectionInfo<ParameterInfo> info = default;
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
            {
                info.Import.Member = parameters[index];

                if (!IsValid(info.Import.Member, ref context)) return arguments;

                GetImportInfo(ref info);
                info.Data = ParseData(ref info.Import, data[index]);

                // Check for override
                var @override = context.GetOverride(in info.Import);

                arguments[index] = null != @override
                    ? BuildImport(ref context, in info.Import, ParseData(ref info.Import, @override.Value))
                    : BuildImport(ref context, in info.Import, in info.Data);
            }

            return arguments;
        }

        protected object?[] BuildParameters(ref PipelineContext context, ParameterInfo[] parameters)
        {
            ReflectionInfo<ParameterInfo> info = default;
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
            {
                info.Import.Member = parameters[index];

                if (!IsValid(info.Import.Member, ref context)) return arguments;

                GetImportInfo(ref info);

                // Check for override
                var @override = context.GetOverride(in info.Import);

                arguments[index] = null != @override
                    ? BuildImport(ref context, in info.Import, ParseData(ref info.Import, @override.Value))
                    : BuildImport(ref context, in info.Import, in info.Data);
            }

            return arguments;
        }

        protected object? BuildImport(ref PipelineContext context, in ImportInfo<ParameterInfo> import, in ImportData data)
        {
            if (ImportType.Value == data.DataType) return data.Value;

            ErrorInfo error = default;
            var contract = new Contract(import.ContractType, import.ContractName);
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            local.Target = data.DataType switch
            {
                ImportType.None => context.Container.Resolve(ref local),

                ImportType.Pipeline => local.GetValueRecursively(import.Member,
                    ((ResolveDelegate<PipelineContext>)data.Value!).Invoke(ref local)),

                    // TODO: Requires proper handling
                _ => local.Error("Invalid Import Type"),
            };

            if (local.IsFaulted && import.AllowDefault)
            {
                local.Target = import.Member.HasDefaultValue
                    ? import.Member.DefaultValue
                    : GetDefaultValue(import.Member.ParameterType);
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
