using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo>
    {
        private static ImportType DefaultImportProvider(ref ImportInfo info)
        {
            // Basics
            string? name = null;
            Type   type = info.MemberInfo.ParameterType;
            var result  = ImportType.None;
            info.Source = ImportSource.Any;
            info.Policy = CreationPolicy.Any;
            info.ValueData.Clear();

            // Default value from ParameterInfo
            if (info.MemberInfo.HasDefaultValue)
            {
                info.Default = info.MemberInfo.DefaultValue;
            }
            else
            { 
                info.AllowDefault = false;
            }

            // Process Attributes
            info.Attributes = Unsafe.As<Attribute[]>(info.MemberInfo.GetCustomAttributes(false));
            foreach (var attribute in info.Attributes)
            {
                switch (attribute)
                {
                    case ImportAttribute import:
                        if (import.ContractType is not null) type = import.ContractType;
                        name = import.ContractName;
                        info.Policy = import.RequiredCreationPolicy;
                        info.Source = import.Source;
                        info.AllowDefault |= import.AllowDefault;
                        info.ValueData.Clear();
                        result = ImportType.Attribute;
                        break;

                    case ImportManyAttribute many:
                        if (many.ContractType is not null) type = many.ContractType;
                        name = many.ContractName;
                        info.Policy = many.RequiredCreationPolicy;
                        info.Source = many.Source;
                        info.ValueData.Clear();
                        result = ImportType.Attribute;
                        break;

                    case DefaultValueAttribute @default:
                        info.Default = @default.Value;
                        result = ImportType.Attribute;
                        break;
                }
            }

            info.Contract = new Contract(type, name);

            return result;
        }
    }
}

