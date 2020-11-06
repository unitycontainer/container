using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity.Injection
{
    public abstract class InjectionMemberInfo<TMemberInfo> : InjectionMember<TMemberInfo, object>,
                                                             IReflectionProvider<TMemberInfo>
                                         where TMemberInfo : MemberInfo
    {
        #region Fields

        private readonly Type?   _type;
        private readonly string? _name;
        private readonly bool _optional;

        #endregion


        #region Constructors

        protected InjectionMemberInfo(string member, object data)
            : base(member, data)
        {
            _name = AnyContractName;
        }

        protected InjectionMemberInfo(string member, bool optional)
            : base(member, RegistrationManager.NoValue)
        {
            _name = AnyContractName;
            _optional = optional;
        }

        protected InjectionMemberInfo(string member, Type contractType, bool optional)
            : base(member, RegistrationManager.NoValue)
        {
            _type = contractType;
            _name = AnyContractName;
            _optional = optional;
        }

        protected InjectionMemberInfo(string member, string? contractName, bool optional)
            : base(member, RegistrationManager.NoValue)
        {
            _name = contractName;
            _optional = optional;
        }

        protected InjectionMemberInfo(string member, Type contractType, string? contractName, bool optional)
            : base(member, RegistrationManager.NoValue)
        {
            _type = contractType;
            _name = contractName;
            _optional = optional;
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract Type MemberType(TMemberInfo info);

        public ImportType FillReflectionInfo(ref ReflectionInfo<TMemberInfo> reflectionInfo)
        {
            if (Data is IReflectionProvider<TMemberInfo> provider)
                return provider.FillReflectionInfo(ref reflectionInfo);
            
            // Optional
            reflectionInfo.Import.AllowDefault |= _optional;

            // Type
            if (Data is Type target && typeof(Type) != MemberType(reflectionInfo.Import.Element))
            {
                reflectionInfo.Import.ContractType = target;
                reflectionInfo.Import.AllowDefault |= _optional;
                reflectionInfo.Data = default;
                return ImportType.None;
            }
            
            if (null != _type) reflectionInfo.Import.ContractType = _type;


            // Name
            if (!ReferenceEquals(_name, AnyContractName)) reflectionInfo.Import.ContractName = _name;

            // Data
            reflectionInfo.Data = ReferenceEquals(RegistrationManager.NoValue, Data) 
                ? default
                : ToImportData(reflectionInfo.Import.Element);

            return reflectionInfo.Data.DataType;
        }

        protected abstract ImportData ToImportData(TMemberInfo memberInfo);

        #endregion
    }
}
