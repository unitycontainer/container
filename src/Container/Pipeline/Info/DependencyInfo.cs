using System;
using System.ComponentModel.Composition;
using Unity.Resolution;

namespace Unity.Container
{
    public struct DependencyInfo<TInfo>
    {
        public TInfo Info;
        public bool AllowDefault;
        public Contract Contract;
        public ImportAttribute? Import;
        public InjectionInfo Injected;

        public DependencyInfo(TInfo info, Type type, bool allowDefault = false)
        {
            Info = info;
            Import = default;
            AllowDefault = allowDefault;
            Contract = new Contract(type);
            Injected = default;
        }

        public DependencyInfo(TInfo info, Type type, ImportAttribute? import, bool allowDefault = false)
        {
            Info = info;
            Import = import; 
            AllowDefault = allowDefault;
            Contract = new Contract(type, import?.ContractName);
            Injected = default;
        }

        public DependencyInfo(TInfo info, Type type, object? data, bool allowDefault = false)
        {
            Info = info;
            Import = default;
            AllowDefault = allowDefault;
            Contract = new Contract(type);
            Injected = data switch
            {
                IResolve iResolve                         => new InjectionInfo((ResolveDelegate<PipelineContext>)iResolve.Resolve,      InjectionType.Resolver),
                ResolveDelegate<PipelineContext> resolver => new InjectionInfo(data,                                                    InjectionType.Resolver),
                IResolverFactory<TInfo> infoFactory       => new InjectionInfo(infoFactory.GetResolver<PipelineContext>(Info),          InjectionType.Resolver),
                IResolverFactory<Type> typeFactory        => new InjectionInfo(typeFactory.GetResolver<PipelineContext>(Contract.Type), InjectionType.Resolver),
                _                                         => new InjectionInfo(data,                                                    InjectionType.Value   ),
            };
        }

        public DependencyInfo(TInfo info, Type type, ImportAttribute? import, object? data, bool allowDefault = false)
        {
            Info = info;
            Import = import;
            AllowDefault = allowDefault;
            Contract = new Contract(type, import?.ContractName);
            Injected = data switch
            {
                IResolve iResolve                         => new InjectionInfo((ResolveDelegate<PipelineContext>)iResolve.Resolve,      InjectionType.Resolver),
                ResolveDelegate<PipelineContext> resolver => new InjectionInfo(data,                                                    InjectionType.Resolver),
                IResolverFactory<TInfo> infoFactory       => new InjectionInfo(infoFactory.GetResolver<PipelineContext>(Info),          InjectionType.Resolver),
                IResolverFactory<Type> typeFactory        => new InjectionInfo(typeFactory.GetResolver<PipelineContext>(Contract.Type), InjectionType.Resolver),
                _                                         => new InjectionInfo(data,                                                    InjectionType.Value   ),
            };
        }
    }
}
