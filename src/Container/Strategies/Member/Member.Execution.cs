using System;
using Unity.Dependency;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        protected void FromContainer<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> descriptor)
            where TContext : IBuilderContext
        {
            ErrorDescriptor error = default;

            descriptor.ValueData[ImportType.Value] = descriptor.AllowDefault
                ? context.FromContract(new Contract(descriptor.ContractType, descriptor.ContractName), ref error)
                : context.FromContract(new Contract(descriptor.ContractType, descriptor.ContractName));

            if (error.IsFaulted)
            {
                if (descriptor.DefaultData.IsValue)
                    descriptor.ValueData[ImportType.Value] = descriptor.DefaultData.Value;
                else
                    descriptor.ValueData = default;
            }
        }

        protected virtual void FromUnknown<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> descriptor)
            where TContext : IBuilderContext
        {
            do
            {
                switch (descriptor.ValueData.Value)
                {
                    case IImportProvider<TMember> provider:
                        descriptor.ValueData.Value = UnityContainer.NoValue;
                        provider.ProvideImport(ref descriptor);
                        break;

                    case IImportProvider provider:
                        descriptor.ValueData.Value = UnityContainer.NoValue;
                        provider.ProvideImport(ref descriptor);
                        break;

                    case IResolve iResolve:
                        descriptor.ValueData.Value = context.FromPipeline(
                            new Contract(descriptor.ContractType, descriptor.ContractName),
                                (ResolveDelegate<TContext>)iResolve.Resolve);
                        break;

                    case ResolveDelegate<TContext> resolver:
                        descriptor.ValueData.Value = context.FromPipeline(
                            new Contract(descriptor.ContractType, descriptor.ContractName), resolver);
                        break;

                    case IResolverFactory<TMember> factory:
                        descriptor.ValueData[ImportType.Value] = context.FromPipeline(
                            new Contract(descriptor.ContractType, descriptor.ContractName),
                            factory.GetResolver<TContext>(descriptor.MemberInfo));
                        return;

                    case IResolverFactory<Type> factory:
                        descriptor.ValueData[ImportType.Value] = context.FromPipeline(
                            new Contract(descriptor.ContractType, descriptor.ContractName),
                            factory.GetResolver<TContext>(descriptor.MemberType));
                        return;

                    case PipelineFactory<TContext> factory:
                        descriptor.ValueData[ImportType.Value] = context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                            factory(ref context));
                        return;

                    case Type target when typeof(Type) != descriptor.MemberType:
                        descriptor.ContractType = target;
                        descriptor.ContractName = null;
                        descriptor.AllowDefault = false;
                        FromContainer(ref context, ref descriptor);
                        return;

                    case UnityContainer.InvalidValue _:                        
                        FromContainer(ref context, ref descriptor);
                        return;

                    default:
                        descriptor.ValueData.Type = ImportType.Value;
                        return;
                }
            }
            while (ImportType.Unknown == descriptor.ValueData.Type);
        }
    }
}
