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
            ResolverOverride? @override;

            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                var import = new MemberDescriptor<ParameterInfo>(parameters[index]);

                DescribeParameter(ref import);

                try
                {
                    import.ValueData = Build(ref context, ref import, data[index]);
                    while (ImportType.Dynamic == import.ValueData.Type)
                        Analyse(ref context, ref import);
                }
                catch (Exception ex)
                {
                    import.Pipeline = (ResolveDelegate<TContext>)((ref TContext context) => context.Error(ex.Message));
                }

                // Use override if provided
                if (0 < context.Overrides.Length && null != (@override = context.GetOverride<ParameterInfo, MemberDescriptor<ParameterInfo>>(ref import)))
                    import.ValueData = Build(ref context, ref import, @override.Value);

                var result = import.ValueData.Type switch
                    {
                        ImportType.None => FromContainer(ref context, ref import),
                        ImportType.Value => import.ValueData,
                        ImportType.Dynamic => FromUnknown(ref context, ref import, import.ValueData.Value),
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
            ResolverOverride? @override;

            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                var import = new MemberDescriptor<ParameterInfo>(parameters[index]);

                DescribeParameter(ref import);

                // Use override if provided
                if (null != (@override = context.GetOverride<ParameterInfo, MemberDescriptor<ParameterInfo>>(ref import)))
                    import.ValueData = Build(ref context, ref import, @override.Value);

                var result = import.ValueData.Type switch
                {
                    ImportType.None => FromContainer(ref context, ref import),
                    ImportType.Value => import.ValueData,
                    ImportType.Dynamic => FromUnknown(ref context, ref import, import.ValueData.Value),
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


        protected ImportData FromPipeline<TContext>(ref TContext context, ref MemberDescriptor<ParameterInfo> import, ResolveDelegate<TContext> pipeline)
            where TContext : IBuilderContext => new ImportData(context.FromPipeline(new Contract(import.ContractType, import.ContractName), pipeline), ImportType.Value);

        protected ImportData FromUnknown<TContext>(ref TContext context, ref MemberDescriptor<ParameterInfo> import, object? data)
            where TContext : IBuilderContext
        {
            do
            {
                switch (data)
                {
                    case IImportProvider<ParameterInfo> provider:
                        import.ValueData.Type = ImportType.None;
                        provider.ProvideImport<TContext, MemberDescriptor<ParameterInfo>>(ref import);
                        break;

                    case IImportProvider provider:
                        import.ValueData.Type = ImportType.None;
                        provider.ProvideImport<TContext, MemberDescriptor<ParameterInfo>>(ref import);
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
                        import.ContractType = target;
                        import.ContractName = null;
                        import.AllowDefault = false;
                        return FromContainer(ref context, ref import);

                    case UnityContainer.InvalidValue _:
                        return FromContainer(ref context, ref import);

                    default:
                        return new ImportData(data, ImportType.Value);
                }
            }
            while (ImportType.Dynamic == import.ValueData.Type);

            return import.ValueData.Type switch
            {
                ImportType.None => FromContainer(ref context, ref import),
                ImportType.Pipeline => new ImportData(((ResolveDelegate<TContext>)import.ValueData.Value!)(ref context), ImportType.Value),
                _ => import.ValueData
            };
        }

    }
}
