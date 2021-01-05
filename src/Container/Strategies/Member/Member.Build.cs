using System;
using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        protected ImportData Build<TContext>(ref TContext context, ref ImportInfo<TDependency> import)
            where TContext : IBuilderContext
        {
            // Local context
            ErrorInfo error = default;
            var contract = new Contract(import.ContractType, import.ContractName);
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            // Process/Resolve data
            local.Target = import.ValueData.Type switch
            {
                ImportType.None => context.Container.Resolve(ref local),

                ImportType.Pipeline => local.GetValueRecursively(import.MemberInfo,
                    ((ResolveDelegate<PipelineContext>)import.ValueData.Value!).Invoke(ref local)),

                // TODO: Requires proper handling
                _ => local.Error("Invalid Import Type"),
            };

            // Check for errors
            if (local.IsFaulted)
            {
                // Set nothing if no default
                if (!import.AllowDefault) return default;

                // Default value
                return import.DefaultData;
            }

            return new ImportData(local.Target, ImportType.Value);
        }

        protected ImportData Build<TContext>(ref TContext context, ref ImportInfo<TDependency> import, object? @override)
            where TContext : IBuilderContext
        {
            // Local context
            ErrorInfo error = default;
            var contract = new Contract(import.ContractType, import.ContractName);
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            // Process/Resolve data
            local.Target = import.ValueData.Type switch
            {
                ImportType.None => context.Container.Resolve(ref local),

                ImportType.Pipeline => local.GetValueRecursively(import.MemberInfo,
                    ((ResolveDelegate<PipelineContext>)import.ValueData.Value!).Invoke(ref local)),

                // TODO: Requires proper handling
                _ => local.Error("Invalid Import Type"),
            };

            // Check for errors
            if (local.IsFaulted)
            {
                // Set nothing if no default
                if (!import.AllowDefault) return default;

                // Default value
                return import.DefaultData;
            }

            return new ImportData(local.Target, ImportType.Value);
        }


        protected ImportData FromData<TContext>(ref TContext context, ref ImportInfo<TDependency> import)
            where TContext : IBuilderContext
        {
            do
            {
                switch (import.ValueData.Value)
                {
                    case IImportDescriptionProvider<TDependency> provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref import);
                        break;

                    case IImportDescriptionProvider provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref import);
                        break;

                    case IResolve iResolve:
                        import.Value = iResolve.Resolve(ref context);
                        return import.ValueData;

                    case ResolveDelegate<TContext> resolver:
                        import.Value = resolver(ref context);
                        return import.ValueData;

                    case IResolverFactory<Type> typeFactory:
                        var fromTypePipeline = typeFactory.GetResolver<TContext>(import.MemberType);
                        import.Value = fromTypePipeline(ref context);
                        return import.ValueData;

                    //case PipelineFactory<TContext> factory:
                    //    var pipeline = typeFactory.GetResolver<TContext>(import.MemberType);
                    //    import.Value = pipeline(ref context);
                    //    return import.ValueData;
                    //    info.Pipeline = factory(info.MemberType);
                    //    return;

                    case Type target when typeof(Type) != import.MemberType:
                        import.ContractType = target;
                        import.AllowDefault = false;
                        import.ValueData.Type = ImportType.None;
                        return Build(ref context, ref import);

                    case UnityContainer.InvalidValue _:
                        import.Value = context.Resolve(import.ContractType, import.ContractName);
                        return import.ValueData;

                    default:
                        import.ValueData.Type = ImportType.Value;
                        return import.ValueData;
                }
            }
            while (ImportType.Unknown == import.ValueData.Type);

            return import.ValueData.Type switch
            {
                ImportType.None     => Build(ref context, ref import),
                ImportType.Pipeline => new ImportData(((ResolveDelegate<TContext>)import.ValueData.Value!)(ref context), ImportType.Value),
                _ => import.ValueData
            };
        }
    }
}
