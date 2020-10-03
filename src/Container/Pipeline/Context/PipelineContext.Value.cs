using System;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineContext
    {
        public object? GetValue<TDependency>(TDependency info, object? value)
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
