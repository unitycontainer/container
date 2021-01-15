using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;


namespace Unity.Container
{
    internal static partial class Providers
    {
        public static void DefaultConstructorImportProvider<TInfo>(ref TInfo descriptor)
            where TInfo : IImportMemberDescriptor<ConstructorInfo>
        {
            // Process Attributes
            descriptor.Attributes = Unsafe.As<Attribute[]>(descriptor.MemberInfo.GetCustomAttributes(false));
            foreach (var attribute in descriptor.Attributes)
            {
                switch (attribute)
                {
                    case ImportingConstructorAttribute:
                        descriptor.IsImport = true;
                        break;
                }
            }
        }
    }
}


