using System;
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

                DescribeParameter(ref import);

                // Injection Data
                import.Dynamic = data[index];

                // Use override if provided
                if (0 < context.Overrides.Length && null != (@override = context.GetOverride<ParameterInfo, ImportDescriptor<ParameterInfo>>(ref import)))
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
                DescribeParameter(ref import);

                // Use override if provided
                if (null != (@override = context.GetOverride<ParameterInfo, ImportDescriptor<ParameterInfo>>(ref import)))
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



        protected ImportData FromContainer<TContext, TMemeber>(ref TContext context, ref ImportDescriptor<ParameterInfo> import)
            where TContext : IBuilderContext
        {
            ErrorDescriptor error = default;

            var value = import.AllowDefault
                ? context.FromContract(import.Contract, ref error)
                : context.FromContract(import.Contract);

            if (error.IsFaulted)
            {
                // Set nothing if no default
                if (!import.AllowDefault) return default;

                // Default value
                return import.DefaultData.IsValue
                    ? new ImportData(import.DefaultData.Value, ImportType.Value)
                    : default;
            }

            return new ImportData(value, ImportType.Value);
        }


        protected ImportData FromPipeline<TContext>(ref TContext context, ref ImportDescriptor<ParameterInfo> import, ResolveDelegate<TContext> pipeline)
            where TContext : IBuilderContext => new ImportData(context.FromPipeline(import.Contract, pipeline), ImportType.Value);

        protected ImportData FromUnknown<TContext>(ref TContext context, ref ImportDescriptor<ParameterInfo> import, object? data)
            where TContext : IBuilderContext
        {
            do
            {
                switch (data)
                {
                    case IImportDescriptionProvider<ParameterInfo> provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref import);
                        break;

                    case IImportDescriptionProvider provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref import);
                        break;

                    case IResolve iResolve:
                        return FromPipeline(ref context, ref import, iResolve.Resolve);

                    case ResolveDelegate<TContext> resolver:
                        return FromPipeline(ref context, ref import, resolver);

                    case IResolverFactory<Type> typeFactory:
                        return FromPipeline(ref context, ref import, typeFactory.GetResolver<TContext>(import.MemberType));

                    case PipelineFactory<TContext> factory:
                        return FromPipeline(ref context, ref import, factory(ref context));

                    case Type target when typeof(Type) != import.MemberType:
                        import.Contract = new Contract(target);
                        import.AllowDefault = false;
                        return FromContainer(ref context, ref import);

                    case UnityContainer.InvalidValue _:
                        return FromContainer(ref context, ref import);

                    default:
                        return new ImportData(data, ImportType.Value);
                }
            }
            while (ImportType.Unknown == import.ValueData.Type);

            return import.ValueData.Type switch
            {
                ImportType.None => FromContainer(ref context, ref import),
                ImportType.Pipeline => new ImportData(((ResolveDelegate<TContext>)import.ValueData.Value!)(ref context), ImportType.Value),
                _ => import.ValueData
            };
        }

    }
}
