using System;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// Base class for generic type parameters.
    /// </summary>
    public abstract class GenericParameterBase : ParameterValue,
                                                 IResolverFactory<Type>,
                                                 IResolverFactory<ParameterInfo>
    {
        #region Fields

        private readonly bool    _isArray;
        private readonly bool    _optional;
        private readonly string? _contractName;
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
            _contractName = contractName;
            _optional = optional;
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

        public override MatchRank Match(ParameterInfo parameter)
        {
            if (!parameter.Member.DeclaringType!.IsGenericType)
                return MatchRank.NoMatch;

            var definition = parameter.Member.DeclaringType!.GetGenericTypeDefinition();
            var type = MethodBase.GetMethodFromHandle(((MethodBase)parameter.Member).MethodHandle, definition.TypeHandle)!
                                 .GetParameters()[parameter.Position]
                                 .ParameterType;
            return Match(type);
        }

        public override MatchRank Match(Type type)
        {
            if (false == _isArray)
                return type.IsGenericParameter && type.Name == _genericParameterName
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch; 

            if (!type.IsArray) return MatchRank.NoMatch;

            return _genericParameterName.Equals(type.GetElementType()!.Name) 
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch;
        }

        #endregion


        #region IResolverFactory

        // TODO: Remove type parameter
        public virtual ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IResolveContext => GetResolver<TContext>(type, _contractName);

        public virtual ResolveDelegate<TContext> GetResolver<TContext>(ParameterInfo info)
            where TContext : IResolveContext => GetResolver<TContext>(info.ParameterType, _contractName);

        #endregion


        #region Implementation

        public override ImportType FillReflectionInfo(ref ReflectionInfo<ParameterInfo> reflectionInfo)
        {
            // Name
            if (!ReferenceEquals(_contractName, InjectionMember.AnyContractName)) 
                reflectionInfo.Import.ContractName = _contractName;

            // Data
            var resolver = GetResolver<PipelineContext>(reflectionInfo.Import.Element);
            reflectionInfo.Data = new ImportData(resolver, ImportType.Pipeline);

            return reflectionInfo.Data.DataType;
        }

        protected virtual ResolveDelegate<TContext> GetResolver<TContext>(Type type, string? name)
            where TContext : IResolveContext => (ref TContext context) => context.Resolve(type, name);

        #endregion
    }
}
