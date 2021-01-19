using System;

namespace Unity.Extension
{
    public delegate void ImportDescriptionProvider<TDescriptor>(ref TDescriptor descriptor)
        where TDescriptor : IImportMemberDescriptor;

    public delegate void ImportDescriptionProvider<TMemberInfo, TDescriptor>(ref TDescriptor descriptor)
        where TDescriptor : IImportMemberDescriptor<TMemberInfo>;


    public interface IImportDescriptionProvider
    {
        void DescribeImport<TContext, TDescriptor>(ref TDescriptor descriptor)
            where TDescriptor : IImportMemberDescriptor
            where TContext    : IBuilderContext;
    }


    public interface IImportDescriptionProvider<TMemberInfo> : IImportDescriptionProvider
    {
        new void DescribeImport<TContext, TDescriptor>(ref TDescriptor descriptor)
            where TDescriptor : IImportMemberDescriptor<TMemberInfo>
            where TContext    : IBuilderContext;
    }
}
