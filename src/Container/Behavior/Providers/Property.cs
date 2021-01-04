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
        public static void DefaultPropertyImportProvider<TInfo>(ref TInfo descriptor)
            where TInfo : IImportDescriptor<PropertyInfo>
        {
            // Basics
            string? name = null;
            Type type = descriptor.MemberInfo.PropertyType;

            // Process Attributes
            descriptor.Attributes = Unsafe.As<Attribute[]>(descriptor.MemberInfo.GetCustomAttributes(false));
            foreach (var attribute in descriptor.Attributes)
            {
                switch (attribute)
                {
                    case ImportAttribute import:
                        descriptor.IsImport = true;
                        if (import.ContractType is not null) type = import.ContractType;
                        name = import.ContractName;
                        descriptor.Policy = import.RequiredCreationPolicy;
                        descriptor.Source = import.Source;
                        descriptor.AllowDefault |= import.AllowDefault;
                        break;

                    case ImportManyAttribute many:
                        descriptor.IsImport = true;
                        if (many.ContractType is not null) type = many.ContractType;
                        name = many.ContractName;
                        descriptor.Policy = many.RequiredCreationPolicy;
                        descriptor.Source = many.Source;
                        break;

                    case DefaultValueAttribute @default:
                        descriptor.IsImport = true;
                        descriptor.Default = @default.Value;
                        break;
                }
            }

            descriptor.ContractType = type;
            descriptor.ContractName = name;
        }
    }
}
