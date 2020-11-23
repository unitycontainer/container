using System;
using System.Reflection;

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

        public override void GetImportInfo<TImport>(ref TImport import)
        {
            // Optional
            import.AllowDefault = _optional;

            // Type
            if (Data is Type target && typeof(Type) != import.MemberType)
            {
                import.ContractType = target;
                return;
            }

            if (null != _type) import.ContractType = _type;

            // Name
            if (!ReferenceEquals(_name, AnyContractName)) import.ContractName = _name;

            // Data
            if (!ReferenceEquals(RegistrationManager.NoValue, Data))
            {
                if (Data is IInjectionProvider provider)
                    provider.GetImportInfo(ref import);
                else
                    import.External = Data;
            }
        }

        #endregion
    }
}
