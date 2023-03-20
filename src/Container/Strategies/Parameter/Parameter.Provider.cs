using System.ComponentModel;
using System.Reflection;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo> : IInjectionProvider<ParameterInfo>
    {
        void IInjectionProvider<ParameterInfo>.ProvideInfo<TDescriptor>(ref TDescriptor descriptor)
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

                    //case ImportManyAttribute many:
                    //    if (many.ContractType is not null)
                    //        descriptor.ContractType = many.ContractType;

                    //    descriptor.ContractName = many.ContractName;
                    //    descriptor.Policy = many.RequiredCreationPolicy;
                    //    descriptor.Source = many.Source;
                    //    descriptor.IsImport = true;
                    //    break;

                    case DefaultValueAttribute @default:
                        descriptor.IsImport = true;
                        descriptor.Default = @default.Value;
                        break;
                }
            }
        }
    }
}
