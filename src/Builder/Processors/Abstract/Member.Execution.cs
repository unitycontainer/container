using Unity.Container;
using Unity.Injection;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TContext, TMemberInfo, TData>
    {
        protected void FromContainer<TMember>(ref TContext context, ref InjectionInfoStruct<TMember> descriptor)
        {
            ErrorDescriptor error = default;

            descriptor.DataValue[Storage.ValueType.Value] = descriptor.AllowDefault
                ? context.FromContract(new Contract(descriptor.ContractType, descriptor.ContractName), ref error)
                : context.FromContract(new Contract(descriptor.ContractType, descriptor.ContractName));

            if (error.IsFaulted)
            {
                if (descriptor.DefaultValue.IsValue)
                    descriptor.DataValue[Storage.ValueType.Value] = descriptor.DefaultValue.Value;
                else
                    descriptor.DataValue = default;
            }
        }

        protected virtual void FromUnknown<TMember>(ref TContext context, ref InjectionInfoStruct<TMember> descriptor)
        {
            do
            {
                switch (descriptor.DataValue.Value)
                {
                    case IInjectionInfoProvider<TMember> provider:
                        descriptor.DataValue.Value = UnityContainer.NoValue;
                        provider.ProvideInfo(ref descriptor);
                        break;

                    case IInjectionInfoProvider provider:
                        descriptor.DataValue.Value = UnityContainer.NoValue;
                        provider.ProvideInfo(ref descriptor);
                        break;

                    case IResolve iResolve:
                        descriptor.DataValue.Value = context.FromPipeline(
                            new Contract(descriptor.ContractType, descriptor.ContractName),
                                (ResolveDelegate<TContext>)iResolve.Resolve<TContext>);
                        break;

                    case ResolveDelegate<TContext> resolver:
                        descriptor.DataValue.Value = context.FromPipeline(
                            new Contract(descriptor.ContractType, descriptor.ContractName), resolver);
                        break;

                    case IResolverFactory<TMember> factory:
                        descriptor.DataValue[Storage.ValueType.Value] = context.FromPipeline(
                            new Contract(descriptor.ContractType, descriptor.ContractName),
                            factory.GetResolver<TContext>(descriptor.MemberInfo));
                        return;

                    case IResolverFactory<Type> factory:
                        descriptor.DataValue[Storage.ValueType.Value] = context.FromPipeline(
                            new Contract(descriptor.ContractType, descriptor.ContractName),
                            factory.GetResolver<TContext>(descriptor.MemberType));
                        return;

                    case ResolverFactory<TContext> factory:
                        descriptor.DataValue[Storage.ValueType.Value] = context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                            factory(descriptor.ContractType));
                        return;

                    case PipelineFactory<TContext> factory:
                        descriptor.DataValue[Storage.ValueType.Value] = context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
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
                        descriptor.DataValue.Type = Storage.ValueType.Value;
                        return;
                }
            }
            while (Storage.ValueType.Unknown == descriptor.DataValue.Type);
        }
    }
}
