using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.Injection
{
    public abstract class InjectionMemberInfo<TMemberInfo> : InjectionMember<TMemberInfo, object>
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

        public override ImportData GetReflectionInfo(ref ImportInfo<TMemberInfo> info)
        {
            // Optional
            info.AllowDefault |= _optional;

            // Type
            if (Data is Type target && typeof(Type) != MemberType(info.Member))
            {
                info.ContractType = target;
                info.AllowDefault |= _optional;
                return default;
            }
            
            if (null != _type) info.ContractType = _type;

            // Name
            if (!ReferenceEquals(_name, AnyContractName)) info.ContractName = _name;

            // Data
            return ReferenceEquals(RegistrationManager.NoValue, Data) 
                ? default
                : new ImportData(Data, ImportType.Unknown);
        }

        #endregion
    }
}
