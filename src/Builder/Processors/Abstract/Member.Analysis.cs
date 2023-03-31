using Unity.Extension;
using Unity.Injection;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TContext, TMemberInfo, TData>
    {
        protected virtual void AnalyzeInfo<TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
        {
            do
            {
                switch (info.DataValue.Value)
                {
                    case IInjectionInfoProvider<TMember> provider:
                        info.DataValue = default;
                        provider.ProvideInfo(ref info);
                        break;

                    case IInjectionInfoProvider provider:
                        info.DataValue = default;
                        provider.ProvideInfo(ref info);
                        break;

                    case IResolve iResolve:
                        info.DataValue[DataType.Unknown] = context.FromPipeline(
                            new Contract(info.ContractType, info.ContractName),
                            (ResolveDelegate<TContext>)iResolve.Resolve<TContext>);
                        break;

                    case ResolveDelegate<TContext> resolver:
                        info.DataValue[DataType.Unknown] = context.FromPipeline(
                            new Contract(info.ContractType, info.ContractName), resolver);
                        break;

                    case PipelineDelegate<TContext> resolver:
                        info.DataValue[DataType.Unknown] = context.FromPipeline(
                            new Contract(info.ContractType, info.ContractName), resolver);
                        break;

                    case IResolverFactory<TMember> factory:
                        info.DataValue[DataType.Unknown] = context.FromPipeline(
                            new Contract(info.ContractType, info.ContractName),
                            factory.GetResolver<TContext>(info.MemberInfo));
                        break;

                    case IResolverFactory<Type> factory:
                        info.DataValue[DataType.Unknown] = context.FromPipeline(
                            new Contract(info.ContractType, info.ContractName),
                            factory.GetResolver<TContext>(info.MemberType));
                        break;

                    case ResolverFactory<TContext> factory:
                        info.DataValue[DataType.Unknown] = context.FromPipeline(
                            new Contract(info.ContractType, info.ContractName),
                            factory(info.ContractType));
                        break;

                    case PipelineFactory<TContext> factory:
                        info.DataValue[DataType.Unknown] = context.FromPipeline(
                            new Contract(info.ContractType, info.ContractName),
                            factory(ref context));
                        break;

                    case Type target when typeof(Type) != info.MemberType:
                        info.ContractType = target;
                        info.ContractName = null;
                        info.AllowDefault = false;
                        info.DefaultValue = default;
                        info.DataValue = default;
                        return;

                    case UnityContainer.InvalidValue _:
                        info.DefaultValue = default;
                        return;

                    case null  when DataType.None  == info.DataValue.Type:
                    case Array when DataType.Array == info.DataValue.Type:
                        return;

                    default:
                        info.DataValue.Type = DataType.Value;
                        return;
                }
            }
            while (DataType.Unknown == info.DataValue.Type);
        }
    }
}
