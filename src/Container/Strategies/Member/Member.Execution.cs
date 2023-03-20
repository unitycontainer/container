using System;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        protected ImportData FromContainer<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> descriptor)
            where TContext : IBuilderContext
        {
            ErrorDescriptor error = default;

            var value = descriptor.AllowDefault
                ? context.FromContract(new Contract(descriptor.ContractType, descriptor.ContractName), ref error)
                : context.FromContract(new Contract(descriptor.ContractType, descriptor.ContractName));

            if (error.IsFaulted)
            {
                // TODO: Default value
                return descriptor.DefaultData.IsValue
                    ? new ImportData(descriptor.DefaultData.Value, ImportType.Value)
                    : default;
            }

            return new ImportData(value, ImportType.Value);
        }

        protected virtual ImportData FromDynamic<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> descriptor)
            where TContext : IBuilderContext
        {
            do
            {
                switch (descriptor.ValueData.Value)
                {
                    case IImportProvider<TMember> provider:
                        descriptor.ValueData.Value = UnityContainer.NoValue;
                        provider.ProvideImport<TContext, MemberDescriptor<TContext, TMember>>(ref descriptor);
                        break;

                    case IImportProvider provider:
                        descriptor.ValueData.Value = UnityContainer.NoValue;
                        provider.ProvideImport<TContext, MemberDescriptor<TContext, TMember>>(ref descriptor);
                        break;

                    case IResolve iResolve:
                        return new ImportData(context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                                (ResolveDelegate<TContext>)iResolve.Resolve), ImportType.Value);

                    case ResolveDelegate<TContext> resolver:
                        return new ImportData(context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                            resolver), ImportType.Value);

                    case IResolverFactory<TMember> factory:
                        return new ImportData(context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                            factory.GetResolver<TContext>(descriptor.MemberInfo)), ImportType.Value);

                    case IResolverFactory<Type> factory:
                        return new ImportData(context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                            factory.GetResolver<TContext>(descriptor.MemberType)), ImportType.Value);

                    case PipelineFactory<TContext> factory:
                        return new ImportData(context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                            factory(ref context)), ImportType.Value);

                    case Type target when typeof(Type) != descriptor.MemberType:
                        descriptor.ContractType = target;
                        descriptor.ContractName = null;
                        descriptor.AllowDefault = false;
                        return FromContainer(ref context, ref descriptor);

                    case UnityContainer.InvalidValue _:
                        return FromContainer(ref context, ref descriptor);

                    default:
                        return new ImportData(descriptor.ValueData.Value, ImportType.Value);
                }
            }
            while (ImportType.Dynamic == descriptor.ValueData.Type);

            return descriptor.ValueData;
        }
    }
}
