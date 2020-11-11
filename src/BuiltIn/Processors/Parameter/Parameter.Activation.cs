using System;
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
            ImportInfo import = default;
            ResolverOverride? @override;

            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                import.MemberInfo = parameters[index];
                
                // TODO: requires optimization
                if (!IsValid(import.MemberInfo, ref context)) return arguments;

                LoadImportInfo(ref import);
                ParseDataImport!(ref import, data[index]);

                // Use override if provided
                if (null != (@override = context.GetOverride(in import)))
                    ParseDataImport!(ref import, @override.Value);

                import.UpdateHashCode();

                var result = import.Data.IsValue
                    ? import.Data
                    : Build(ref context, ref import);

                if (context.IsFaulted) return arguments;

                // TODO: requires optimization
                arguments[index] = !result.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : result.Value;
            }

            return arguments;
        }

        protected object?[] Build(ref PipelineContext context, ParameterInfo[] parameters)
        {
            ImportInfo import = default;
            ResolverOverride? @override;

            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                import.MemberInfo = parameters[index];

                // TODO: requires optimization
                if (!IsValid(import.MemberInfo, ref context)) return arguments;

                // Load attributes
                LoadImportInfo(ref import);

                // Use override if provided
                if (null != (@override = context.GetOverride(in import)))
                    ParseDataImport!(ref import, @override.Value);

                import.UpdateHashCode();

                var result = import.Data.IsValue
                    ? import.Data
                    : Build(ref context, ref import);

                if (context.IsFaulted) return arguments;

                // TODO: requires optimization
                arguments[index] = !result.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : result.Value;
            }

            return arguments;
        }

        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? GetDefaultValue(Type t)
            => (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                ? Activator.CreateInstance(t) : null;

        #endregion
    }
}
