using Unity.Container;
using Unity.Extension;
using Unity.Injection;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TContext, TMemberInfo, TDependency, TData>
    {
        protected void FromContainer<TMember>(ref TContext context, ref MemberInjectionInfo<TMember> descriptor)
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

        protected virtual void FromUnknown<TMember>(ref TContext context, ref MemberInjectionInfo<TMember> descriptor)
        {
            do
            {
                switch (descriptor.ValueData.Value)
                {
                    case IInjectionInfoProvider<TMember> provider:
                        descriptor.ValueData.Value = UnityContainer.NoValue;
                        provider.ProvideInfo(ref descriptor);
                        break;

                    case IInjectionInfoProvider provider:
                        descriptor.ValueData.Value = UnityContainer.NoValue;
                        provider.ProvideInfo(ref descriptor);
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

                    case ResolverFactory<TContext> factory:
                        descriptor.ValueData[ImportType.Value] = context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                            factory(descriptor.ContractType));
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
