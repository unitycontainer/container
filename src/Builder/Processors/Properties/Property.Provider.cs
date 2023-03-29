using System.ComponentModel;

namespace Unity.Processors
{
    public partial class PropertyProcessor<TContext>
    {
        protected override void InjectionInfoProvider<TDescriptor>(ref TDescriptor descriptor)
        {
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
