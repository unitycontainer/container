using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo>
    {
        protected object?[] Build<TContext>(ref TContext context, ParameterInfo[] parameters, object?[] data)
            where TContext : IBuilderContext
        {
            ImportInfo<ParameterInfo> import = default;
            ResolverOverride? @override;

            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                import.MemberInfo = parameters[index];
                
                // TODO: requires optimization
                if (!IsValid(ref context, import.MemberInfo)) return arguments;

                DescribeImport(ref import);

                // Use override if provided
                if (null != (@override = GetOverride(ref context, in import)))
                    import.Dynamic = @override.Value;
                else
                    import.Dynamic = data[index];

                var result = import.ValueData.Type switch
                {
                    ImportType.Value => import.ValueData,
                    ImportType.None  => Build(ref context, ref import),
                    _                => FromData(ref context, ref import)
                };

                if (context.IsFaulted) return arguments;

                // TODO: requires optimization
                arguments[index] = !result.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : result.Value;
            }

            return arguments;
        }

        protected object?[] Build<TContext>(ref TContext context, ParameterInfo[] parameters)
            where TContext : IBuilderContext
        {
            ImportInfo<ParameterInfo> import = default;
            ResolverOverride? @override;

            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                import.MemberInfo = parameters[index];

                // TODO: requires optimization
                if (!IsValid(ref context, import.MemberInfo)) return arguments;

                // Get Import descriptor
                DescribeImport(ref import);

                // Use override if provided
                if (null != (@override = GetOverride(ref context, in import)))
                    import.Dynamic = @override.Value;

                var result = import.ValueData.Type switch
                {
                    ImportType.Value => import.ValueData,
                    ImportType.None  => Build(ref context, ref import),
                    _                => FromData(ref context, ref import)
                };

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
