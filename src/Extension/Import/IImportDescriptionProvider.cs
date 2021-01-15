using System;

namespace Unity.Extension
{
    public delegate void ImportDescriptionProvider<TDescriptor>(ref TDescriptor descriptor)
        where TDescriptor : IImportMemberDescriptor;

    public delegate void ImportDescriptionProvider<TMemberInfo, TDescriptor>(ref TDescriptor descriptor)
        where TDescriptor : IImportMemberDescriptor<TMemberInfo>;


    public interface IImportDescriptionProvider
    {
        void DescribeImport<TDescriptor>(ref TDescriptor descriptor)
        where TDescriptor : IImportMemberDescriptor;
    }


    public interface IImportDescriptionProvider<TMemberInfo> : IImportDescriptionProvider
    {
        new void DescribeImport<TDescriptor>(ref TDescriptor descriptor)
        where TDescriptor : IImportMemberDescriptor<TMemberInfo>;
    }
}
