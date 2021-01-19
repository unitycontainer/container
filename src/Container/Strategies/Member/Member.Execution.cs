using System;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        protected virtual void Execute<TContext>(ref TContext context, ref MemberDescriptor<TMemberInfo> import)
            where TContext : IBuilderContext
        {
            ResolverOverride? @override;

            var result = 0 < context.Overrides.Length && null != (@override = context.GetOverride<TMemberInfo, MemberDescriptor<TMemberInfo>>(ref import))
                ? FromUnknown(ref context, ref import, @override.Value)
                : import.ValueData.Type switch
                {
                    ImportType.None => FromContainer(ref context, ref import),
                    ImportType.Value => import.ValueData,
                    ImportType.Dynamic => FromUnknown(ref context, ref import, import.ValueData.Value),
                    ImportType.Pipeline => FromPipeline(ref context, ref import, (ResolveDelegate<TContext>)import.ValueData.Value!), // TODO: Switch to Contract
                    _ => default
                };

            if (context.IsFaulted || !result.IsValue) return;

            try
            {
                Execute(import.MemberInfo, context.Existing!, result.Value);
            }
            catch (ArgumentException ex)
            {
                context.Error(ex.Message);
            }
            catch (Exception exception)
            {
                context.Capture(exception);
            }
        }

        protected ImportData FromContainer<TContext, TMemeber>(ref TContext context, ref MemberDescriptor<TMemeber> import)
            where TContext : IBuilderContext
        {
            ErrorDescriptor error = default;

            var value = import.AllowDefault
                ? context.FromContract(new Contract(import.ContractType, import.ContractName), ref error)
                : context.FromContract(new Contract(import.ContractType, import.ContractName));

            if (error.IsFaulted)
            {
                // TODO: Default value
                return import.DefaultData.IsValue
                    ? new ImportData(import.DefaultData.Value, ImportType.Value)
                    : default;
            }

            return new ImportData(value, ImportType.Value);
        }


        protected ImportData FromPipeline<TContext>(ref TContext context, ref MemberDescriptor<TMemberInfo> import, ResolveDelegate<TContext> pipeline)
            where TContext : IBuilderContext => new ImportData(context.FromPipeline(new Contract(import.ContractType, import.ContractName), pipeline), ImportType.Value);

        protected virtual ImportData FromUnknown<TContext>(ref TContext context, ref MemberDescriptor<TMemberInfo> import, object? data)
            where TContext : IBuilderContext
        {
            do
            {
                switch (data)
                {
                    case IImportDescriptionProvider<TMemberInfo> provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport<TContext, MemberDescriptor<TMemberInfo>>(ref import);
                        break;

                    case IImportDescriptionProvider provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport<TContext, MemberDescriptor<TMemberInfo>>(ref import);
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
