using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Extension;


namespace Unity.Container
{
    internal static partial class Providers
    {
        public static void DefaultConstructorImportProvider<TInfo>(ref TInfo descriptor)
            where TInfo : IImportDescriptor<ConstructorInfo>
        {
            if (descriptor.MemberInfo.IsDefined(typeof(ImportingConstructorAttribute)))
                descriptor.IsImport = true;
        }
    }
}


