using System.Reflection;
using Unity.Extension;


namespace Unity.Container
{
    internal static partial class Providers
    {
        public static void DefaultMethodImportProvider<TInfo>(ref TInfo descriptor)
            where TInfo : IImportMemberDescriptor<MethodInfo>
        {
            if (descriptor.MemberInfo.IsDefined(typeof(InjectionMethodAttribute)))
                descriptor.IsImport = true;
        }
    }
}


