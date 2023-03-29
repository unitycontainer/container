using System.ComponentModel;
using System.Reflection;
using Unity.Injection;



namespace Unity.Processors
{

    public abstract partial class ParameterProcessor<TContext, TMemberInfo> 
    {
        private static void ParameterInfoProvider<TDescriptor>(ref TDescriptor descriptor)
            where TDescriptor : IInjectionInfo<ParameterInfo>
        {
            // Default value from ParameterInfo
            if (descriptor.MemberInfo.HasDefaultValue)
                descriptor.Default = descriptor.MemberInfo.DefaultValue;

            // Process Attributes
            foreach (var attribute in descriptor.MemberInfo.GetCustomAttributes(false))
            {
                switch (attribute)
                {
                    case DependencyResolutionAttribute import:
                        if (import.ContractType is not null)
                            descriptor.ContractType = import.ContractType;

                        descriptor.ContractName = import.ContractName;
                        descriptor.AllowDefault |= import.AllowDefault;
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
