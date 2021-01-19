using System;

namespace Unity.Extension
{
    public delegate void ImportProvider<TDescriptor>(ref TDescriptor descriptor)
        where TDescriptor : IImportDescriptor;

    public delegate void ImportProvider<TMemberInfo, TDescriptor>(ref TDescriptor descriptor)
        where TDescriptor : IImportDescriptor<TMemberInfo>;


    public interface IImportProvider
    {
        void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
            where TDescriptor : IImportDescriptor
            where TContext    : IBuilderContext;
    }


    public interface IImportProvider<TMemberInfo> : IImportProvider
    {
        new void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
            where TDescriptor : IImportDescriptor<TMemberInfo>
            where TContext    : IBuilderContext;
    }
}
