using System;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TMemberInfo, TData>
    {
        protected virtual void AnalyzeInfo<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
            where TContext : IBuildPlanContext
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
                        FromPipeline(ref context, ref info, iResolve.Resolve);
                        break;

                    case ResolverPipeline resolver:
                        FromPipeline(ref context, ref info, resolver);
                        break;

                    case IResolverFactory<TMember> factory:
                        FromPipeline(ref context, ref info,
                            factory.GetResolver<BuilderContext>(info.MemberInfo));
                        break;

                    case IResolverFactory<Type> factory:
                        FromPipeline(ref context, ref info, factory.GetResolver<BuilderContext>(info.MemberType));
                        break;

                    case ResolverFactory<BuilderContext> factory:
                        FromPipeline(ref context, ref info, factory(info.ContractType));
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
            while (!context.IsFaulted && DataType.Unknown == info.DataValue.Type);
        }

        private void FromPipeline<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info, ResolverPipeline @delegate)
            where TContext : IBuildPlanContext
        {
            var request = new BuilderContext.RequestInfo();
            var contract = new Contract(info.ContractType, info.ContractName);
            var builderContext = request.Context(context.Container, ref contract);

            try
            { 
                info.DataValue[DataType.Unknown] = @delegate(ref builderContext);
            } 
            catch (Exception exception) 
            {
                context.Capture(exception);
            }
        }
    }
}
