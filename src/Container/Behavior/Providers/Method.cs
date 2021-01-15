using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;


namespace Unity.Container
{
    internal static partial class Providers
    {
        public static void DefaultMethodImportProvider<TInfo>(ref TInfo descriptor)
            where TInfo : IImportMemberDescriptor<MethodInfo>
        {
            // Process Attributes
            descriptor.Attributes = Unsafe.As<Attribute[]>(descriptor.MemberInfo.GetCustomAttributes(false));
            foreach (var attribute in descriptor.Attributes)
            {
                switch (attribute)
                {
                    case InjectionMethodAttribute:
                        descriptor.IsImport = true;
                        break;
                }
            }
        }
    }
}


