using System;
using System.Reflection;
using Unity.Container;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that overrides
    /// the value injected whenever there is a dependency of the
    /// given type, regardless of where it appears in the object graph.
    /// </summary>
    public class DependencyOverride : ResolverOverride, 
                                      IMatchImport<ParameterInfo>,
                                      IMatchImport<FieldInfo>, 
                                      IMatchImport<PropertyInfo>
    {
        #region Fields

        protected readonly Type? Type;

        #endregion


        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// the given contract type with the given value.
        /// </summary>
        /// <param name="contractType">Type of the <see cref="Contract"/></param>
        /// <param name="value">Value to override with</param>
        public DependencyOverride(Type contractType, object? value)
            : base(Contract.AnyContractName, value, MatchRank.Compatible) => Type = contractType;

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given name
        /// </summary>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        /// <param name="value">Value to override with</param>
        public DependencyOverride(string contractName, object? value)
            : base(contractName, value, MatchRank.Compatible)
        {
        }

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type and a name
        /// </summary>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        /// <param name="contractType">Type of the <see cref="Contract"/></param>
        /// <param name="value">Value to override with</param>
        public DependencyOverride(Type contractType, string? contractName, object? value)
            : base(contractName, value, MatchRank.Compatible) => Type = contractType;

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependency on specific type matching the given type and a name
        /// </summary>
        /// <param name="targetType">Target <see cref="Type"/> to override dependency on</param>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        /// <param name="contractType">Type of the <see cref="Contract"/></param>
        /// <param name="value">Value to override with</param>
        public DependencyOverride(Type? targetType, Type contractType, string? contractName, object? value)
            : base(targetType, contractName, value, MatchRank.Compatible) => Type = contractType;

        #endregion


        #region  Match

        private MatchRank MatchContract(Type contractType, string? contractName)
        {
            if (!ReferenceEquals(Contract.AnyContractName, Name) && (contractName != Name))
                return MatchRank.NoMatch;

            // If Type is 'null', all types are compatible
            if (Type is null) return Value.MatchTo(contractType);

            // Matches exactly
            if (contractType == Type) return MatchRank.ExactMatch;

            // Can be assigned to
            if (contractType.IsAssignableFrom(Type)) return MatchRank.HigherProspect;

            return MatchRank.NoMatch;
        }


        public MatchRank Matches(ParameterInfo member, Type contractType, string? contractName) 
            => null != Target && member.Member.DeclaringType != Target
            ? MatchRank.NoMatch
            : MatchContract(contractType, contractName);

        public MatchRank Matches(FieldInfo field, Type contractType, string? contractName)
            => null != Target && field.DeclaringType != Target
            ? MatchRank.NoMatch
            : MatchContract(contractType, contractName);

        public MatchRank Matches(PropertyInfo property, Type contractType, string? contractName)
            => null != Target && property.DeclaringType != Target 
            ? MatchRank.NoMatch
            : MatchContract(contractType, contractName);


        #endregion
    }

    #region Generic

    /// <summary>
    /// A convenience version of <see cref="DependencyOverride"/> that lets you
    /// specify the dependency type using generic syntax.
    /// </summary>
    /// <typeparam name="T">Type of the dependency to override.</typeparam>
    public class DependencyOverride<T> : DependencyOverride
    {
        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type and a name
        /// </summary>
        /// <remarks>
        /// This constructor creates an override that will match with any
        /// target type as long as the dependency type and name match. To 
        /// target specific type use <see cref="ResolverOverride.OnType(Type)"/> 
        /// method.
        /// </remarks>
        /// <param name="target">Target type to override dependency on</param>
        /// <param name="name">Name of the dependency</param>
        /// <param name="value">Override value</param>
        public DependencyOverride(Type target, string name, object value)
            : base(target, typeof(T), name, value)
        {
        }

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type and a name
        /// </summary>
        /// <remarks>
        /// This constructor creates an override that will match with any
        /// target type as long as the dependency type and name match. To 
        /// target specific type use <see cref="ResolverOverride.OnType(Type)"/> 
        /// method.
        /// </remarks>
        /// <param name="name">Name of the dependency</param>
        /// <param name="value">Override value</param>
        public DependencyOverride(string name, object value)
            : base(null, typeof(T), name, value)
        {
        }

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type
        /// </summary>
        /// <remarks>
        /// This constructor creates an override that will match with any
        /// target type as long as the dependency type match. To 
        /// target specific type use <see cref="ResolverOverride.OnType(Type)"/> 
        /// method.
        /// </remarks>
        /// <param name="value">Override value</param>
        public DependencyOverride(object value)
            : base(null, typeof(T), null, value)
        {
        }
    }

    #endregion
}
