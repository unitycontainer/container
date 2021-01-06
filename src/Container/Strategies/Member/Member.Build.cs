using System;
using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        protected ImportData FromContainer<TContext>(ref TContext context, ref ImportInfo<TDependency> import)
            where TContext : IBuilderContext
        {
            ErrorInfo error   = default;
            Contract contract = new Contract(import.ContractType, import.ContractName);  // TODO: Optimize
            
            var local = import.AllowDefault
                ? context.CreateContext<TContext>(ref contract, ref error)
                : context.CreateContext<TContext>(ref contract);

            local.Target = local.Resolve();

            if (local.IsFaulted)
            {
                // Set nothing if no default
                if (!import.AllowDefault) return default;

                // Default value
                return GetDefault(ref import);
            }

            return new ImportData(local.Target, ImportType.Value);
        }


        protected ImportData FromPipeline<TContext>(ref TContext context, ref ImportInfo<TDependency> import, ResolveDelegate<TContext> pipeline)
            where TContext : IBuilderContext
        {
            // Local context
            var contract = new Contract(import.ContractType, import.ContractName);
            
            var local = context.CreateContext<TContext>(ref contract);

            //local.Target = pipeline(ref local);

            return new ImportData(local.Target, ImportType.Value);
        }


        protected ImportData FromUnknown<TContext>(ref TContext context, ref ImportInfo<TDependency> import, object? data)
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
                        return FromPipeline(ref context, ref import, iResolve.Resolve);

                    case ResolveDelegate<TContext> resolver:
                        return FromPipeline(ref context, ref import, resolver);

                    case IResolverFactory<Type> typeFactory:
                        return FromPipeline(ref context, ref import, typeFactory.GetResolver<TContext>(import.MemberType));

                    case PipelineFactory<TContext> factory:
                        return FromPipeline(ref context, ref import, factory(ref context));

                    case Type target when typeof(Type) != import.MemberType:
                        import.ContractType = target;
                        import.AllowDefault = false;
                        return FromContainer(ref context, ref import);

                    case UnityContainer.InvalidValue _:
                        return FromContainer(ref context, ref import);

                    default:
                        return new ImportData(import.ValueData, ImportType.Value);
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
