using System.Reflection;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo>
    {
        protected object?[] BuildUp<TContext>(ref TContext context, ParameterInfo[] parameters, object?[] data)
            where TContext : IBuilderContext
        {
            ImportDescriptor<ParameterInfo> import = default;
            ResolverOverride? @override;

            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                import.MemberInfo = parameters[index];
                
                // TODO: requires optimization
                if (!IsValid(ref context, import.MemberInfo)) return arguments;

                DescribeImport(ref import);

                // Injection Data
                import.Dynamic = data[index];

                // Use override if provided
                if (0 < context.Overrides.Length && null != (@override = GetOverride(ref context, ref import)))
                    import.Dynamic = @override.Value;

                var result = import.ValueData.Type switch
                    {
                        ImportType.None => FromContainer(ref context, ref import),
                        ImportType.Value => import.ValueData,
                        ImportType.Unknown => FromUnknown(ref context, ref import, import.ValueData.Value),
                        ImportType.Pipeline => FromPipeline(ref context, ref import, (ResolveDelegate<TContext>)import.ValueData.Value!), // TODO: Switch to Contract
                        _ => default
                    }; ;


                if (context.IsFaulted) return arguments;

                // TODO: requires optimization
                arguments[index] = !result.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : result.Value;
            }

            return arguments;
        }

        protected object?[] BuildUp<TContext>(ref TContext context, ParameterInfo[] parameters)
            where TContext : IBuilderContext
        {
            ImportDescriptor<ParameterInfo> import = default;
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
                if (null != (@override = GetOverride(ref context, ref import)))
                    import.Dynamic = @override.Value;

                var result = import.ValueData.Type switch
                {
                    ImportType.None => FromContainer(ref context, ref import),
                    ImportType.Value => import.ValueData,
                    ImportType.Unknown => FromUnknown(ref context, ref import, import.ValueData.Value),
                    ImportType.Pipeline => FromPipeline(ref context, ref import, (ResolveDelegate<TContext>)import.ValueData.Value!), // TODO: Switch to Contract
                    _ => default
                }; ;

                if (context.IsFaulted) return arguments;

                // TODO: requires optimization
                arguments[index] = !result.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : result.Value;
            }

            return arguments;
        }

    }
}
