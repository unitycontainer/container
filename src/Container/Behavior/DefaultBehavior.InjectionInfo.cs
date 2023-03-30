using System.ComponentModel;
using System.Reflection;
using Unity.Injection;

namespace Unity.Container
{
    internal static partial class UnityDefaultBehaviorExtension
    {
        static void ConstructorInjectionInfoProvider<TInjectionInfo>(ref TInjectionInfo info)
            where TInjectionInfo : IInjectionInfo<ConstructorInfo>
        {
            if (info.MemberInfo.IsDefined(typeof(InjectionConstructorAttribute)))
                info.IsImport = true;
        }

        static void MethodInjectionInfoProvider<TInjectionInfo>(ref TInjectionInfo info)
            where TInjectionInfo : IInjectionInfo<MethodInfo>
        {
            if (info.MemberInfo.IsDefined(typeof(InjectionMethodAttribute)))
                info.IsImport = true;
        }

        static void ParameterInjectionInfoProvider<TInjectionInfo>(ref TInjectionInfo info)
            where TInjectionInfo : IInjectionInfo<ParameterInfo>
        {
            // Default value from ParameterInfo
            if (info.MemberInfo.HasDefaultValue)
                info.Default = info.MemberInfo.DefaultValue;

            // Process Attributes
            foreach (var attribute in info.MemberInfo.GetCustomAttributes(false))
            {
                switch (attribute)
                {
                    case DependencyResolutionAttribute import:
                        if (import.ContractType is not null)
                            info.ContractType = import.ContractType;

                        info.ContractName = import.ContractName;
                        info.AllowDefault |= import.AllowDefault;
                        info.IsImport = true;
                        break;

                    case DefaultValueAttribute @default:
                        info.IsImport = true;
                        info.Default = @default.Value;
                        break;
                }
            }
        }

        static void FieldInjectionInfoProvider<TInjectionInfo>(ref TInjectionInfo info)
            where TInjectionInfo : IInjectionInfo<FieldInfo>
        {
            // Process Attributes
            foreach (var attribute in info.MemberInfo.GetCustomAttributes(false))
            {
                switch (attribute)
                {
                    case DependencyResolutionAttribute import:
                        if (import.ContractType is not null)
                            info.ContractType = import.ContractType;

                        info.ContractName = import.ContractName;
                        info.AllowDefault |= import.AllowDefault;
                        info.IsImport = true;
                        break;

                    case DefaultValueAttribute @default:
                        info.IsImport = true;
                        info.Default = @default.Value;
                        break;
                }
            }
        }

        static void PropertyInjectionInfoProvider<TInjectionInfo>(ref TInjectionInfo info)
            where TInjectionInfo : IInjectionInfo<PropertyInfo>
        {
            // Process Attributes
            foreach (var attribute in info.MemberInfo.GetCustomAttributes(false))
            {
                switch (attribute)
                {
                    case DependencyResolutionAttribute import:
                        if (import.ContractType is not null)
                            info.ContractType = import.ContractType;

                        info.ContractName = import.ContractName;
                        info.AllowDefault |= import.AllowDefault;
                        info.IsImport = true;
                        break;

                    case DefaultValueAttribute @default:
                        info.IsImport = true;
                        info.Default = @default.Value;
                        break;
                }
            }
        }
    }
}
