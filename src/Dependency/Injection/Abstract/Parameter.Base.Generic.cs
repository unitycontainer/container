using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Dependency;

namespace Unity.Injection
{
    /// <summary>
    /// Base class for generic type parameters.
    /// </summary>
    [DebuggerDisplay("{GetType().Name}: Type={ParameterTypeName} Contract={ContractName ?? \"null\"}")]
    public abstract class GenericParameterBase : ParameterValue,
                                                 IMatchInfo<FieldInfo>,
                                                 IMatchInfo<PropertyInfo>
    {
        #region Fields

        protected readonly bool    AllowDefault;
        protected readonly string? ContractName;

        private readonly bool    _isArray;
        private readonly string  _genericParameterName;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="contractName">Name of the contract</param>
        protected GenericParameterBase(string genericParameterName, string? contractName, bool optional)
        {
            if (genericParameterName is null) throw new ArgumentNullException(nameof(genericParameterName));

            if (genericParameterName.EndsWith("[]", StringComparison.Ordinal) ||
                genericParameterName.EndsWith("()", StringComparison.Ordinal))
            {
                _genericParameterName = genericParameterName.Replace("[]", string.Empty).Replace("()", string.Empty);
                _isArray = true;
            }
            else
            {
                _genericParameterName = genericParameterName;
                _isArray = false;
            }
            ContractName = contractName;
            AllowDefault = optional;
        }


        #endregion


        #region Public Properties

        /// <summary>
        /// Name for the type represented by this <see cref="ParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public virtual string ParameterTypeName => _genericParameterName;

        #endregion


        #region  IMatch

        public virtual MatchRank RankMatch(FieldInfo field)
        {
            if (!field.DeclaringType!.IsGenericType)
                return MatchRank.NoMatch;

            var type = field.DeclaringType!
                            .GetGenericTypeDefinition()
                            .GetField(field.Name)!
                            .FieldType;

            return RankMatch(type);
        }

        public virtual MatchRank RankMatch(PropertyInfo property)
        {
            if (!property.DeclaringType!.IsGenericType)
                return MatchRank.NoMatch;

            var type = property.DeclaringType!
                               .GetGenericTypeDefinition()
                               .GetProperty(property.Name)!
                               .PropertyType;
            return RankMatch(type);
        }

        public override MatchRank RankMatch(ParameterInfo parameter)
        {
            if (!parameter.Member.DeclaringType!.IsGenericType)
                return MatchRank.NoMatch;

            var definition = parameter.Member.DeclaringType!.GetGenericTypeDefinition();
            var type = MethodBase.GetMethodFromHandle(((MethodBase)parameter.Member).MethodHandle, definition.TypeHandle)!
                                 .GetParameters()[parameter.Position]
                                 .ParameterType;
            return RankMatch(type);
        }

        protected override MatchRank RankMatch(Type type)
        {
            if (!_isArray)
                return type.IsGenericParameter && type.Name == _genericParameterName
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch; 

            if (!type.IsArray) return MatchRank.NoMatch;

            return _genericParameterName.Equals(type.GetElementType()!.Name) 
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch;
        }

        #endregion


        #region Import


        public override void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
        {
            if (!ReferenceEquals(ContractName, Contract.AnyContractName))
                descriptor.ContractName = ContractName;

            // Optional
            descriptor.AllowDefault = AllowDefault;
        }


        #endregion

        public override string ToString()
            => $"{GetType().Name}: Type={ParameterTypeName} Contract={ContractName ?? "null"}";
    }
}
