﻿using Unity.Dependency;
using Unity.Resolution;

namespace Unity.Extension
{
    public delegate void ImportProvider<TDescriptor>(ref TDescriptor descriptor)
        where TDescriptor : IInjectionInfo;

    public delegate void ImportProvider<TMemberInfo, TDescriptor>(ref TDescriptor descriptor)
        where TDescriptor : IInjectionInfo<TMemberInfo>;


    public interface IImportProvider
    {
        void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
            where TDescriptor : IInjectionInfo
            where TContext    : IBuilderContext;
    }


    public interface IImportProvider<TMemberInfo> : IImportProvider
    {
        new void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
            where TDescriptor : IInjectionInfo<TMemberInfo>
            where TContext    : IBuilderContext;
    }
}
