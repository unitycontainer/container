﻿using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo> : IImportProvider<ParameterInfo>
    {
        void IImportProvider<ParameterInfo>.ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
        {
            // Default value from ParameterInfo
            if (descriptor.MemberInfo.HasDefaultValue)
                descriptor.Default = descriptor.MemberInfo.DefaultValue;

            // Process Attributes
            foreach (var attribute in descriptor.MemberInfo.GetCustomAttributes(false))
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