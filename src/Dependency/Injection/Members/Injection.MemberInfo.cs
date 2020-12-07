using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity.Injection
{
    public abstract class InjectionMemberInfo<TMemberInfo> : InjectionMember<TMemberInfo, object>
                                         where TMemberInfo : MemberInfo
    {
        #region Fields

        private readonly Type?   _contractType;
        private readonly string? _contractName;
        private readonly bool _optional;

        #endregion


        #region Constructors

        protected InjectionMemberInfo(string member, object data, bool optional)
            : base(member, data)
        {
            _contractName = Contract.AnyContractName;
            _optional = optional;
        }

        protected InjectionMemberInfo(string member, bool optional)
            : base(member, RegistrationManager.NoValue)
        {
            _contractName = Contract.AnyContractName;
            _optional = optional;
        }

        protected InjectionMemberInfo(string member, Type contractType, string? contractName, bool optional)
            : base(member, RegistrationManager.NoValue)
        {
            _contractType = contractType;
            _contractName = contractName;
            _optional = optional;
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        public override MatchRank Match(TMemberInfo other)
            => other.Name != Name
                ? MatchRank.NoMatch
                : ReferenceEquals(Data, RegistrationManager.NoValue)
                    ? MatchRank.ExactMatch
                    : MatchRank.Compatible;

        public override void GetImportInfo<TImport>(ref TImport import)
        {
            if (Data is IInjectionProvider provider)
            { 
                provider.GetImportInfo(ref import);
                return;
            }

            // Optional
            import.AllowDefault = _optional;

            // Type
            if (Data is Type target && typeof(Type) != import.MemberType)
            {
                import.ContractType = target;
                return;
            }

            if (null != _contractType) import.ContractType = _contractType;

            // Name
            if (!ReferenceEquals(_contractName, Contract.AnyContractName)) import.ContractName = _contractName;

            // Data
            if (!ReferenceEquals(RegistrationManager.NoValue, Data)) import.External = Data;
        }

        #endregion
    }
}
