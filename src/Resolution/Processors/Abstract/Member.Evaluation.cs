using Unity.Builder;
using Unity.Injection;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TMemberInfo, TData>
    {
        protected static void EvaluateInfo<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
            where TContext : IBuildPlanContext
        {
            do
            {
                switch (info.InjectedValue.Value)
                {
                    case IInjectionInfoProvider<TMember> provider:
                        info.InjectedValue = default;
                        provider.ProvideInfo(ref info);
                        break;

                    case IInjectionInfoProvider provider:
                        info.InjectedValue = default;
                        provider.ProvideInfo(ref info);
                        break;

                    case IResolve iResolve:
                        info.InjectedValue[DataType.Pipeline] = (ResolverPipeline)iResolve.Resolve;
                        return;
                        
                    case ResolverPipeline resolver:
                        info.InjectedValue[DataType.Pipeline] = resolver;
                        return;

                    case IResolverFactory<TMember> factory:
                        info.InjectedValue[DataType.Pipeline] = factory.GetResolver<BuilderContext>(info.MemberInfo);
                        return;

                    case IResolverFactory<Type> factory:
                        info.InjectedValue[DataType.Pipeline] = factory.GetResolver<BuilderContext>(info.MemberType);
                        return;

                    case ResolverFactory<BuilderContext> factory:
                        info.InjectedValue[DataType.Pipeline] = factory(info.ContractType);
                        return;

                    case Type target when typeof(Type) != info.MemberType:
                        info.ContractType = target;
                        info.ContractName = null;
                        info.AllowDefault = false;
                        info.DefaultValue = default;
                        info.InjectedValue = default;
                        return;

                    case UnityContainer.InvalidValue _:
                        info.DefaultValue = default;
                        return;

                    case null when DataType.None == info.InjectedValue.Type:
                    case Array when DataType.Array == info.InjectedValue.Type:
                        return;

                    default:
                        info.InjectedValue.Type = DataType.Value;
                        return;
                }
            }
            while (!context.IsFaulted && DataType.Unknown == info.InjectedValue.Type);
        }

        protected static void EvaluateData<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
        where TContext : IBuilderContext
        {
            do
            {
                switch (info.InjectedValue.Value)
                {
                    case IInjectionInfoProvider<TMember> provider:
                        info.InjectedValue = default;
                        provider.ProvideInfo(ref info);
                        break;

                    case IInjectionInfoProvider provider:
                        info.InjectedValue = default;
                        provider.ProvideInfo(ref info);
                        break;

                    case IResolve iResolve:
                        info.InjectedValue[DataType.Value] = iResolve.Resolve(ref context);
                        return;

                    case ResolveDelegate<TContext> resolver:
                        info.InjectedValue[DataType.Value] = resolver(ref context);
                        return;

                    case IResolverFactory<TMember> factory:
                        info.InjectedValue[DataType.Value] = factory.GetResolver<TContext>(info.MemberInfo)(ref context);
                        return;

                    case IResolverFactory<Type> factory:
                        info.InjectedValue[DataType.Value] = factory.GetResolver<TContext>(info.MemberType)(ref context);
                        return;

                    case ResolverFactory<TContext> factory:
                        info.InjectedValue[DataType.Value] = factory(info.ContractType)(ref context);
                        return;

                    case Type target when typeof(Type) != info.MemberType:
                        info.ContractType = target;
                        info.ContractName = null;
                        info.AllowDefault = false;
                        info.DefaultValue = default;
                        info.InjectedValue = default;
                        return;

                    case UnityContainer.InvalidValue _:
                        info.DefaultValue = default;
                        return;

                    case null when DataType.None == info.InjectedValue.Type:
                    case Array when DataType.Array == info.InjectedValue.Type:
                        return;

                    default:
                        info.InjectedValue.Type = DataType.Value;
                        return;
                }
            }
            while (!context.IsFaulted && DataType.Unknown == info.InjectedValue.Type);
        }

    }
}
