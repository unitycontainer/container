using System;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineContext
    {

        public object? Resolve<TDependency>(ref Contract contract, TDependency info, object? data)
        {
            PipelineContext local;

            switch (data)
            {
                case ResolveDelegate<PipelineContext> resolver:
                    local = DependencyContext(ref contract, data);
                    return local.InterpretValue(info, resolver(ref local));

                case IResolve iResolve:
                    local = DependencyContext(ref contract, data);
                    return local.InterpretValue(info, iResolve.Resolve(ref local));

                case IResolverFactory<TDependency> infoFactory:
                    local = DependencyContext(ref contract, data);
                    return local.InterpretValue(info, infoFactory.GetResolver<PipelineContext>(info)
                                                                 .Invoke(ref local));
                case IResolverFactory<Type> typeFactory:
                    local = DependencyContext(ref contract, data);
                    return local.InterpretValue(info, typeFactory.GetResolver<PipelineContext>(Type)
                                                                 .Invoke(ref local));
                default:
                    return Container.ResolveDependency(ref contract, ref this);
            }
        }

        private object? InterpretValue<TDependency>(TDependency info, object? value)
        {
            return value switch
            {
                IResolve iResolve                         => InterpretValue(info, iResolve.Resolve(ref this)),

                ResolveDelegate<PipelineContext> resolver => InterpretValue(info, resolver(ref this)),

                IResolverFactory<TDependency> infoFactory => InterpretValue(info, infoFactory.GetResolver<PipelineContext>(info)
                                                                                             .Invoke(ref this)),
                IResolverFactory<Type> typeFactory        => InterpretValue(info, typeFactory.GetResolver<PipelineContext>(Type)
                                                                                             .Invoke(ref this)),
                _ => value,
            };
        }
    }
}
