using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Providers
    {
        public static void DefaultParameterImportProvider<TInfo>(ref TInfo descriptor)
            where TInfo : IImportMemberDescriptor<ParameterInfo>
        {
            // Default value from ParameterInfo
            if (descriptor.MemberInfo.HasDefaultValue)
                descriptor.Default = descriptor.MemberInfo.DefaultValue;

            // Process Attributes
            descriptor.Attributes = Unsafe.As<Attribute[]>(descriptor.MemberInfo.GetCustomAttributes(false));
            foreach (var attribute in descriptor.Attributes)
            {
                switch (attribute)
                {
                    case ImportAttribute import:
                        if (import.ContractType is not null) 
                            descriptor.ContractType = import.ContractType;

                        descriptor.ContractName = import.ContractName;
                        descriptor.Policy = import.RequiredCreationPolicy;
                        descriptor.Source = import.Source;
                        descriptor.AllowDefault |= import.AllowDefault;
                        descriptor.IsImport = true;
                        break;

                    case ImportManyAttribute many:
                        if (many.ContractType is not null) 
                            descriptor.ContractType = many.ContractType;

                        descriptor.ContractName = many.ContractName;
                        descriptor.Policy = many.RequiredCreationPolicy;
                        descriptor.Source = many.Source;
                        descriptor.IsImport = true;
                        break;

                    case DefaultValueAttribute @default:
                        descriptor.IsImport = true;
                        descriptor.Default = @default.Value;
                        break;
                }
            }
        }
    }
}

