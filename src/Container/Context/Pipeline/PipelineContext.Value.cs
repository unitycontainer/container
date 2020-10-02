using System;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineContext
    {
        public object? GetValue<TDependency>(ref Contract contract, TDependency info, object? data)
        {
            PipelineContext local;

            switch (data)
            {
                case ResolveDelegate<PipelineContext> resolver:
                    local = new PipelineContext(ref this, ref contract, data);
                    return local.GetValue(info, resolver(ref local));

                case IResolve iResolve:
                    local = new PipelineContext(ref this, ref contract, data);
                    return local.GetValue(info, iResolve.Resolve(ref local));

                case IResolverFactory<TDependency> infoFactory:
                    local = new PipelineContext(ref this, ref contract, data);
                    return local.GetValue(info, infoFactory.GetResolver<PipelineContext>(info)
                                                           .Invoke(ref local));
                case IResolverFactory<Type> typeFactory:
                    local = new PipelineContext(ref this, ref contract, data);
                    return local.GetValue(info, typeFactory.GetResolver<PipelineContext>(Type)
                                                           .Invoke(ref local));
                default:
                    return data;
            }
        }

        private object? GetValue<TDependency>(TDependency info, object? value)
        {
            return value switch
            {
                ResolveDelegate<PipelineContext> resolver => GetValue(info, resolver(ref this)),

                IResolve iResolve                         => GetValue(info, iResolve.Resolve(ref this)),

                IResolverFactory<TDependency> infoFactory => GetValue(info, infoFactory.GetResolver<PipelineContext>(info)
                                                                                       .Invoke(ref this)),
                IResolverFactory<Type> typeFactory        => GetValue(info, typeFactory.GetResolver<PipelineContext>(Type)
                                                                                       .Invoke(ref this)),
                _ => value,
            };
        }
    }
}
