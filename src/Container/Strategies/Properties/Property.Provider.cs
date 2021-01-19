using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Unity.Container
{
    public partial class PropertyStrategy
    {
        public override void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
        {
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
