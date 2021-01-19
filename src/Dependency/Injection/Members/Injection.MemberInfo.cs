using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;

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
            : base(member, UnityContainer.NoValue)
        {
            _contractName = Contract.AnyContractName;
            _optional = optional;
        }

        protected InjectionMemberInfo(string member, Type contractType, string? contractName, bool optional)
            : base(member, UnityContainer.NoValue)
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
                : ReferenceEquals(Data, UnityContainer.NoValue)
                    ? MatchRank.ExactMatch
                    : MatchRank.Compatible;


        /// <inheritdoc/>
        public override void DescribeImport<TContext, TDescriptor>(ref TDescriptor descriptor)
        {
            if (Data is IImportDescriptionProvider provider)
            { 
                provider.DescribeImport<TContext, TDescriptor>(ref descriptor);
                return;
            }

            // Optional
            descriptor.AllowDefault = _optional;

            // Type
            if (Data is Type target && typeof(Type) != descriptor.MemberType)
            {
                descriptor.ContractType = target;
                descriptor.ContractName = null;
                return;
            }

            if (_contractType is not null && !ReferenceEquals(descriptor.ContractType, _contractType))
                    descriptor.ContractType = _contractType!;
                
            if (!ReferenceEquals(_contractName, Contract.AnyContractName))
                    descriptor.ContractName = _contractName;

            // Data
            if (!ReferenceEquals(UnityContainer.NoValue, Data)) descriptor.Dynamic = Data;
        }

        #endregion
    }
}
