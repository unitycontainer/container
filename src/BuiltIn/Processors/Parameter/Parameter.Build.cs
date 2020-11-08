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

            ImportInfo<ParameterInfo> import = default;
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
            {
                import.Member = parameters[index];

                if (!IsValid(import.Member, ref context)) return arguments;

                GetImportInfo(ref import);

                // Check for override
                var @override = context.GetOverride(in import);

                arguments[index] = null != @override
                    ? BuildImport(ref context, in import, ParseData(ref import, @override.Value))
                    : BuildImport(ref context, in import, ParseData(ref import, data[index]));
            }

            return arguments;
        }

        protected object?[] BuildParameters(ref PipelineContext context, ParameterInfo[] parameters)
        {
            ImportInfo<ParameterInfo> import = default;
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
            {
                import.Member = parameters[index];

                if (!IsValid(import.Member, ref context)) return arguments;

                GetImportInfo(ref import);

                // Check for override
                var @override = context.GetOverride(in import);

                arguments[index] = null != @override
                    ? BuildImport(ref context, in import, ParseData(ref import, @override.Value))
                    : BuildImport(ref context, in import);
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

        protected object? BuildImport(ref PipelineContext context, in ImportInfo<ParameterInfo> import)
        {
            ErrorInfo error = default;
            var contract = new Contract(import.ContractType, import.ContractName);
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            local.Target = context.Container.Resolve(ref local);

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
